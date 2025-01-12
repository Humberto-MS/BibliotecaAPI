using BibliotecaAPI.DTOs;
using BibliotecaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaAPI.Controllers.V1 {

    [ApiController]
    [Route ( "api/cuentas" )]
    public class CuentasController: ControllerBase {
        private readonly UserManager<IdentityUser> user_manager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> sign_in_manager;
        private readonly HashService hash_service;
        private readonly IDataProtector data_protector;

        public CuentasController (
            UserManager<IdentityUser> user_manager,
            IConfiguration configuration,
            SignInManager<IdentityUser> sign_in_manager,
            IDataProtectionProvider data_protection_provider,
            HashService hash_service
        ) {
            this.user_manager = user_manager;
            this.configuration = configuration;
            this.sign_in_manager = sign_in_manager;
            this.hash_service = hash_service;
            data_protector = data_protection_provider.CreateProtector ( "valor_unico_y_quizas_secreto" );
        }

        [HttpPost ( "registrar", Name = "RegistrarUsuario" )]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar ( CredencialesUsuario credenciales_usuario ) {
            var usuario = new IdentityUser {
                UserName = credenciales_usuario.Email,
                Email = credenciales_usuario.Email
            };

            var result = await user_manager.CreateAsync ( usuario, credenciales_usuario.Password );
            if ( result.Succeeded ) return await ConstruirToken ( credenciales_usuario );
            else return BadRequest ( result.Errors );
        }

        [HttpPost ( "login", Name = "LoginUsuario" )]
        public async Task<ActionResult<RespuestaAutenticacion>> Login ( CredencialesUsuario credenciales_usuario ) {
            var result = await sign_in_manager.PasswordSignInAsync (
                credenciales_usuario.Email,
                credenciales_usuario.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if ( result.Succeeded ) return await ConstruirToken ( credenciales_usuario );
            else return BadRequest ( "Login incorrecto" );
        }

        [HttpGet ( "RenovarToken", Name = "RenovarToken" )]
        [Authorize ( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar () {
            var email_claim = HttpContext.User.Claims.Where ( claim => claim.Type == "email" ).FirstOrDefault ();
            var email = email_claim!.Value;

            var credenciales_usuario = new CredencialesUsuario () {
                Email = email
            };

            return await ConstruirToken ( credenciales_usuario );
        }

        private async Task<RespuestaAutenticacion> ConstruirToken ( CredencialesUsuario credenciales_usuario ) {
            var claims = new List<Claim> () {
                new Claim ( "email", credenciales_usuario.Email )
            };

            var usuario = await user_manager.FindByEmailAsync ( credenciales_usuario.Email );
            var claims_db = await user_manager.GetClaimsAsync ( usuario );
            claims.AddRange ( claims_db );
            var llave = new SymmetricSecurityKey ( Encoding.UTF8.GetBytes ( configuration [ "llavejwt" ]! ) );
            var credentials = new SigningCredentials ( llave, SecurityAlgorithms.HmacSha256 );
            var expiracion = DateTime.UtcNow.AddYears ( 1 );

            var security_token = new JwtSecurityToken (
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiracion,
                signingCredentials: credentials
            );

            return new RespuestaAutenticacion () {
                Token = new JwtSecurityTokenHandler ().WriteToken ( security_token ),
                Expiracion = expiracion
            };
        }

        [HttpPost ( "HacerAdmin", Name = "HacerAdmin" )]
        public async Task<ActionResult> HacerAdmin ( EditarAdminDTO editar_admin_dto ) {
            var usuario = await user_manager.FindByEmailAsync ( editar_admin_dto.Email );
            await user_manager.AddClaimAsync ( usuario, new Claim ( "esAdmin", "1" ) );
            return NoContent ();
        }

        [HttpPost ( "RemoverAdmin", Name = "RemoverAdmin" )]
        public async Task<ActionResult> RemoverAdmin ( EditarAdminDTO editar_admin_dto ) {
            var usuario = await user_manager.FindByEmailAsync ( editar_admin_dto.Email );
            await user_manager.RemoveClaimAsync ( usuario, new Claim ( "esAdmin", "1" ) );
            return NoContent ();
        }
    }
}
