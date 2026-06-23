using FluentValidation;
using DocConverter.Application.DTOs;

namespace DocConverter.Application.Validators;

public class SplitRequestValidator : AbstractValidator<SplitRequest>
{
    public SplitRequestValidator()
    {
        RuleFor(x => x.StartPage)
            .GreaterThanOrEqualTo(1).WithMessage("La página de inicio debe ser mayor o igual a 1.");

        RuleFor(x => x.EndPage)
            .GreaterThanOrEqualTo(x => x.StartPage).WithMessage("La página de fin debe ser mayor o igual a la página de inicio.");
    }
}