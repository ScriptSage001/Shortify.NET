﻿using FluentValidation;

namespace Shortify.NET.Application.Url.Commands.ShortenUrl
{
    internal class ShortenUrlCommandValidator : AbstractValidator<ShortenUrlCommand>
    {
        public ShortenUrlCommandValidator()
        {
            RuleFor(x => x).NotEmpty();

            RuleFor(x => x.Url)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.HttpRequest)
                .NotEmpty()
                .NotNull();
        }
    }
}
