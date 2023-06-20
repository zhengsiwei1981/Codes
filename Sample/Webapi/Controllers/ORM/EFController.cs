using EFTestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Webapi.Controllers.ORM
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EFController : ControllerBase
    {
        [HttpPost]
        public void Add(string name, [FromServices] EFTestModel.EFTestModel dbContext)
        {
            dbContext.TestTables.Add(new TestTable() { Name = name });
            dbContext.SaveChanges();
        }
        [HttpPost]
        public void Update(TestTable testTable, [FromServices] EFTestModel.EFTestModel dbContext)
        {
            var item = dbContext.TestTables.Where(t => t.Id == testTable.Id).FirstOrDefault();
            if (item != null)
            {
                item.Name = testTable.Name;
            }
            dbContext.SaveChanges();
        }
        [HttpGet]
        public void AddChild(int parentId, string name, [FromServices] EFTestModel.EFTestModel dbContext)
        {
            dbContext.TestChildren.Add(new TestChild() { ParentId = parentId, Name = name });
            dbContext.SaveChanges();
        }
        [HttpGet]
        public IList<TestChild> GetList(int parentId, [FromServices] EFTestModel.EFTestModel dbContext)
        {
            var item = dbContext.TestTables.Include(t => t.TestChildren).Single(t => t.Id == parentId);
            return item?.TestChildren!;
        }
    }
}
