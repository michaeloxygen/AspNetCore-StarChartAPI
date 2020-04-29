using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestials = new List<CelestialObject>();
            foreach (var celestialObject in _context.CelestialObjects.Where(c => c.Name == name))
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
                celestials.Add(celestialObject);
            }
            if (!celestials.Any())
            {
                return NotFound();
            }

            return Ok(celestials);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestials = new List<CelestialObject>();
            foreach (var celestialObject in _context.CelestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
                celestials.Add(celestialObject);
            }

            return Ok(celestials);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var readCelestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (readCelestialObject == null)
            {
                return NotFound();
            }

            readCelestialObject.Name = celestialObject.Name;
            readCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            readCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(readCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var readCelestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (readCelestialObject == null)
            {
                return NotFound();
            }

            readCelestialObject.Name = name;
            
            _context.Update(readCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id );
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            
            _context.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
