using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.SingingKeys.Commands;

public class RotateKeyCommandHandler(ISigningKeyService signingKeyService) : IRequestHandler<RotateKeyCommand>
{
    public async Task Handle(RotateKeyCommand request, CancellationToken cancellationToken = default)
    {
        await signingKeyService.RotateKeysAsync(cancellationToken);
    }
}