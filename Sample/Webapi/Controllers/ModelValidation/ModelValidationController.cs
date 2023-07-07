using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Webapi.Controllers.ModelValidation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelValidationController : ControllerBase
    {
        [HttpPost("Insert")]
        [Produces("application/json")]
        public IActionResult Insert(TestValidation testValidation)
        {
            return Ok(testValidation);
        }
        /// <summary>
        /// 远程验证，需要通过jquery ajax调用
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("verify")]
        [Produces("application/json")]
        public IActionResult VerifyUser(User user)
        {
            return Ok(user);
        }
        /// <summary>
        /// 顶级节点验证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("checkId")]
        [Produces("application/json")]
        public IActionResult CheckAge([BindRequired, FromQuery] int id)
        {
            return Ok(id);
        }
    }
    public class User
    {
        [Remote(action: "VerifyName", controller: "RemoteValidation", AdditionalFields = nameof(FirstName))]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Remote(action: "VerifyName", controller: "RemoteValidation", AdditionalFields = nameof(LastName))]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
    public class TestValidation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "不能超过100个字符")]
        public string? Title { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        [NameValidation("myname")]
        public string Name { get; set; }
    }
    public class NameValidationAttribute : ValidationAttribute
    {
        private string _name;
        public NameValidationAttribute(string name)
        {
            this._name = name;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var obj = validationContext.ObjectInstance as TestValidation;
            if (obj.Name != this._name)
            {
                if (this.ErrorMessage != null)
                {
                    return new ValidationResult(this.ErrorMessage);
                }
                else
                {
                    return new ValidationResult("不是指定的名称");
                }
            }
            return ValidationResult.Success;
        }
    }
}
