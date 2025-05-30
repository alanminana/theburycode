using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using theburycode.Models;
using theburycode.Services;
using theburycode.ViewModels;

namespace theburycode.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly TheBuryCodeContext _context;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(IClienteService clienteService, TheBuryCodeContext context, ILogger<ClienteController> logger)
        {
            _clienteService = clienteService;
            _context = context;
            _logger = logger;
        }

        // GET: Cliente
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteService.GetAllAsync();
            var viewModels = clientes.Select(c => MapToViewModel(c)).ToList();
            return View("~/Views/Cliente/Index.cshtml", viewModels);
        }

        // GET: Cliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteService.GetByIdAsync(id.Value);
            if (cliente == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(cliente);
            return View(viewModel);
        }

        // GET: Cliente/Create
        public IActionResult Create()
        {
            LoadViewData();
            return View(new ClienteViewModel());
        }

        // POST: Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Dni,Genero,EstadoCivil,Telefono,Celular,Email,FechaNacimiento,CiudadId,Scoring,ContactoEmergencia,Notas")] ClienteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el DNI ya existe
                if (await _clienteService.ExisteDni(viewModel.Dni))
                {
                    ModelState.AddModelError("Dni", "El DNI ya está registrado");
                    LoadViewData();
                    return View(viewModel);
                }

                try
                {
                    var cliente = MapToEntity(viewModel);
                    cliente.UsuarioAlta = User.Identity?.Name ?? "SYSTEM";

                    await _clienteService.CreateAsync(cliente);
                    TempData["Success"] = "Cliente creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear cliente");
                    ModelState.AddModelError("", "Error al guardar el cliente");
                }
            }

            LoadViewData();
            return View(viewModel);
        }

        // GET: Cliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteService.GetByIdAsync(id.Value);
            if (cliente == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(cliente);
            LoadViewData();
            return View(viewModel);
        }

        // POST: Cliente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Dni,Genero,EstadoCivil,Telefono,Celular,Email,FechaNacimiento,CiudadId,Scoring,ContactoEmergencia,Notas")] ClienteViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el DNI ya existe en otro cliente
                    if (await _clienteService.ExisteDni(viewModel.Dni, id))
                    {
                        ModelState.AddModelError("Dni", "El DNI ya está registrado en otro cliente");
                        LoadViewData();
                        return View(viewModel);
                    }

                    var cliente = await _clienteService.GetByIdAsync(id);
                    if (cliente == null)
                    {
                        return NotFound();
                    }

                    // Actualizar propiedades
                    cliente.Nombre = viewModel.Nombre;
                    cliente.Apellido = viewModel.Apellido;
                    cliente.Dni = viewModel.Dni;
                    cliente.Genero = viewModel.Genero;
                    cliente.EstadoCivil = viewModel.EstadoCivil;
                    cliente.Telefono = viewModel.Telefono;
                    cliente.Celular = viewModel.Celular;
                    cliente.Email = viewModel.Email;
                    cliente.FechaNacimiento = viewModel.FechaNacimiento.HasValue ? DateOnly.FromDateTime(viewModel.FechaNacimiento.Value) : null;
                    cliente.CiudadId = viewModel.CiudadId;
                    cliente.Scoring = viewModel.Scoring;
                    cliente.ContactoEmergencia = viewModel.ContactoEmergencia;
                    cliente.Notas = viewModel.Notas;
                    cliente.UsuarioModificacion = User.Identity?.Name ?? "SYSTEM";

                    await _clienteService.UpdateAsync(cliente);
                    TempData["Success"] = "Cliente actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ClienteExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar cliente");
                    ModelState.AddModelError("", "Error al actualizar el cliente");
                }
            }

            LoadViewData();
            return View(viewModel);
        }

        // GET: Cliente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteService.GetByIdAsync(id.Value);
            if (cliente == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(cliente);
            return View(viewModel);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _clienteService.DeleteAsync(id);
                if (result)
                {
                    TempData["Success"] = "Cliente eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se puede eliminar el cliente porque tiene ventas o créditos asociados";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente");
                TempData["Error"] = "Error al eliminar el cliente";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Cliente/Search
        [HttpGet]
        public async Task<IActionResult> Search(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return Json(new { success = false, message = "DNI requerido" });
            }

            var cliente = await _clienteService.GetByDniAsync(dni);
            if (cliente == null)
            {
                return Json(new { success = false, message = "Cliente no encontrado" });
            }

            return Json(new
            {
                success = true,
                cliente = new
                {
                    id = cliente.Id,
                    nombre = cliente.Nombre,
                    apellido = cliente.Apellido,
                    nombreCompleto = $"{cliente.Apellido}, {cliente.Nombre}",
                    dni = cliente.Dni,
                    scoring = cliente.Scoring,
                    email = cliente.Email,
                    celular = cliente.Celular
                }
            });
        }

        // GET: Cliente/GetCiudades/1
        [HttpGet]
        public async Task<IActionResult> GetCiudades(int provinciaId)
        {
            var ciudades = await _context.Ciudads
                .Where(c => c.ProvinciaId == provinciaId)
                .Select(c => new { value = c.Id, text = c.Nombre })
                .ToListAsync();

            return Json(ciudades);
        }

        private async Task<bool> ClienteExists(int id)
        {
            return await _context.Clientes.AnyAsync(e => e.Id == id);
        }

        private void LoadViewData()
        {
            ViewData["Generos"] = new SelectList(_context.Generos, "Codigo", "Descripcion");
            ViewData["EstadosCiviles"] = new SelectList(_context.EstadoCivils, "Codigo", "Descripcion");
            ViewData["Provincias"] = new SelectList(_context.Provincia, "Id", "Nombre");
            ViewData["Ciudades"] = new SelectList(_context.Ciudads, "Id", "Nombre");
        }

        private ClienteViewModel MapToViewModel(Cliente cliente)
        {
            return new ClienteViewModel
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Dni = cliente.Dni,
                Genero = cliente.Genero,
                EstadoCivil = cliente.EstadoCivil,
                Telefono = cliente.Telefono,
                Celular = cliente.Celular,
                Email = cliente.Email,
                FechaNacimiento = cliente.FechaNacimiento.HasValue ? cliente.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue) : null,
                CiudadId = cliente.CiudadId,
                Scoring = cliente.Scoring,
                ContactoEmergencia = cliente.ContactoEmergencia,
                Notas = cliente.Notas,
                CiudadNombre = cliente.Ciudad?.Nombre,
                ProvinciaNombre = cliente.Ciudad?.Provincia?.Nombre,
                DomicilioParticular = cliente.DomicilioParticular != null ? new DomicilioViewModel
                {
                    CalleYNumero = cliente.DomicilioParticular.CalleYnumero, // Cambiar CalleNumero por CalleYnumero
                    DescripcionDomicilio = cliente.DomicilioParticular.DescripcionDomicilio
                } : null,
                DomicilioLaboral = cliente.DomicilioLaboral != null ? new DomicilioLaboralViewModel
                {
                    NombreEmpresa = cliente.DomicilioLaboral.NombreEmpresa,
                    CalleYNumero = cliente.DomicilioLaboral.CalleYnumero,
                    DescripcionDomicilioLaboral = cliente.DomicilioLaboral.DescripcionDomicilioLaboral,
                    CiudadLaboralId = cliente.DomicilioLaboral.CiudadLaboralId,
                    TelefonoLaboral = cliente.DomicilioLaboral.TelefonoLaboral
                } : null
            };
        }

        private Cliente MapToEntity(ClienteViewModel viewModel)
        {
            return new Cliente
            {
                Id = viewModel.Id,
                Nombre = viewModel.Nombre,
                Apellido = viewModel.Apellido,
                Dni = viewModel.Dni,
                Genero = viewModel.Genero,
                EstadoCivil = viewModel.EstadoCivil,
                Telefono = viewModel.Telefono,
                Celular = viewModel.Celular,
                Email = viewModel.Email,
                FechaNacimiento = viewModel.FechaNacimiento.HasValue ? DateOnly.FromDateTime(viewModel.FechaNacimiento.Value) : null, // Corregir esta línea
                CiudadId = viewModel.CiudadId,
                Scoring = viewModel.Scoring,
                ContactoEmergencia = viewModel.ContactoEmergencia,
                Notas = viewModel.Notas
            };
        }
    }
}