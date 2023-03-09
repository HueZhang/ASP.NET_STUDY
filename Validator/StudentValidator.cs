using FluentValidation;
using Models;

namespace WebApi.Validator
{
    /// <summary>
    /// 
    /// </summary>
    public class StudentValidator : AbstractValidator<Student>
    {
        /// <summary>
        /// 
        /// </summary>
        public StudentValidator()
        {
            RuleFor(it => it.Name).NotEmpty();
        }
    }
}
