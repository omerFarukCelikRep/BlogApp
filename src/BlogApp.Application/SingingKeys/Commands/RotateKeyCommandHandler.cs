namespace BlogApp.Application.SingingKeys.Commands;

public class RotateKeyCommandHandler : IRequestHandler<RotateKeyCommand>
{
    public Task Handle(RotateKeyCommand request, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}