using Chrestin_Project.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Chrestin_Project.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class DepartmentController : ControllerBase
	{
		private readonly Context context; 
		public DepartmentController(Context context)
		{
			this.context = context;
		}
		[HttpGet]
		public IActionResult Getall()
		{
			List<Department> alldeps = context.Departments.ToList();

			return Ok(alldeps);
		}
		[HttpGet]
		[Route("{id:int}")]
		
		public IActionResult GetById (int id)
		{
			Department department = context.Departments.FirstOrDefault(d => d.Id == id);
			if (department != null)
			{
				return Ok(department);

			}
			return NotFound();
		}
		[HttpPost]
		public IActionResult Add(Department dept)
		{
			if(dept != null)
			{
				context.Departments.Add(dept);
				context.SaveChanges();
				return Ok("ADDED");
			}
			return BadRequest();
		}
		[HttpDelete]
		public IActionResult Delete(int id)
		{
			Department dep = context.Departments.FirstOrDefault(dept => dept.Id == id);
			if (dep != null)
			{
				context.Departments.Remove(dep);
				context.SaveChanges();
				return Ok("REMOVED");

			}
			return NotFound(id);
		}
		[HttpPut]
		public IActionResult Update(int id, Department dep)
		{
			Department deptochange = context.Departments.FirstOrDefault(dept => dept.Id == id);
			if (deptochange != null)
			{
				deptochange.Name = dep.Name;
				deptochange.Manager = dep.Manager;	
				context.SaveChanges();
				return Ok("UPDATED");

			}
			return NotFound(id);
		}
	}
}
