using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BuildingService.Handlers // <--- VERIFICA CHE QUESTO NAMESPACE SIA CORRETTO!
{
    public class BypassAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BypassAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Quando questo schema è attivo (cioè, bypass), autentica un utente fittizio.
            var claims = new[] { new Claim(ClaimTypes.Name, "BypassUser", ClaimValueTypes.String, "BypassScheme") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // In modalità bypass, non vogliamo un vero "challenge".
            // Restituisce 200 OK (o 401 se preferisci che il client riceva un errore ma senza interazione)
            Response.StatusCode = 200;
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            // Simile a Challenge, evita di fare un Forbidden "reale" nel bypass.
            Response.StatusCode = 200;
            return Task.CompletedTask;
        }
    }
}