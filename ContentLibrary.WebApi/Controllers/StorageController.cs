using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Models;
using ContentLibrary.WebAPI.Models.Storage;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace ContentLibrary.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public StorageController(IStorageService storageService, IMapper mapper)
        {
            _storageService = storageService;
            _mapper = mapper;
        }

        // GET: api/Storage
        [HttpGet]
        public ActionResult<IEnumerable<StorageViewModel>> Get()
        {
            var storages = _storageService.GetAllStorages();
            return Ok(_mapper.Map<IEnumerable<StorageViewModel>>(storages));
        }

        // GET: api/Storage/5
        [HttpGet("{id}")]
        public ActionResult<StorageViewModel> Get(int id)
        {
            var storage = _storageService.GetStorageById(id);
            if (storage == null)
                return NotFound();

            return Ok(_mapper.Map<StorageViewModel>(storage));
        }

        // GET: api/Storage/ByType/{type}
        [HttpGet("ByType/{type}")]
        public ActionResult<IEnumerable<StorageViewModel>> GetByType(string type)
        {
            if (!Enum.TryParse<StorageTypeDto>(type, out var storageType))
                return BadRequest("Invalid storage type");

            var storages = _storageService.GetStoragesByType(storageType);
            return Ok(_mapper.Map<IEnumerable<StorageViewModel>>(storages));
        }

        // GET: api/Storage/5/AvailableSpace
        [HttpGet("{id}/AvailableSpace")]
        public ActionResult<decimal> GetAvailableSpace(int id)
        {
            var storage = _storageService.GetStorageById(id);
            if (storage == null)
                return NotFound();

            return Ok(_storageService.GetAvailableSpace(id));
        }

        // POST: api/Storage
        [HttpPost]
        public ActionResult<StorageViewModel> Post([FromBody] StorageCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var storageDto = _mapper.Map<StorageDto>(model);
            _storageService.AddStorage(storageDto);

            // Оскільки ми не маємо унікального ідентифікатора для пошуку сховища після створення,
            // ми повертаємо всі сховища, і фронтенд може знайти нове сховище за назвою
            var storages = _storageService.GetAllStorages();
            var createdStorage = storages.FirstOrDefault(s => s.Name == model.Name && s.Location == model.Location);

            if (createdStorage == null)
                return StatusCode(500, "Storage was created but could not be retrieved");

            return CreatedAtAction(nameof(Get), new { id = createdStorage.Id }, _mapper.Map<StorageViewModel>(createdStorage));
        }

        // PUT: api/Storage/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] StorageUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingStorage = _storageService.GetStorageById(id);
            if (existingStorage == null)
                return NotFound();

            var storageDto = _mapper.Map<StorageDto>(model);
            storageDto.Id = id;
            storageDto.UsedSpace = existingStorage.UsedSpace; // Збереження поточного використаного місця
            _storageService.UpdateStorage(storageDto);

            return NoContent();
        }

        // DELETE: api/Storage/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var storage = _storageService.GetStorageById(id);
            if (storage == null)
                return NotFound();

            _storageService.DeleteStorage(id);
            return NoContent();
        }
    }
}