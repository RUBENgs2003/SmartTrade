using SmartTrade.Entities;
using SmartTrade.Logica.Services;
using SmartTrade.Persistencia.DataAccess;
using SmartTrade.Views;

namespace SmartTrade.Tests
{ 
    public class Tests
    {
        STService service = new STService(new STDAL(new SupabaseContext()));
        private Usuario usuario;
        private Perfil perfil;

        string contrase�aPrevia;
        string emailPrevio;

        [SetUp]
        public async Task Setup()
        {
                this.usuario = await service.GetUsuarioById("usuariotest");
                if (usuario == null) Assert.Fail("No se pudo encontrar al usuario con el nick proporcionado");
                this.contrase�aPrevia = this.usuario.Password;
                this.emailPrevio = this.usuario.Email;
                
                perfil = new Perfil();
        }

        [TestCase("nuevaContrase�a", "nuevoEmail")]
        public async Task TestCambiarUsuario(string contrase�aEsperada, string emailEsperado)
        {
            if (contrase�aEsperada == contrase�aPrevia || emailEsperado == emailPrevio)
            {
                Assert.Fail("�La contrase�a esperada y la contrase�a previa son iguales!");
            }

            perfil.ModificarContrase�a(contrase�aEsperada);
            perfil.ModificarEmail(emailEsperado);

            service.SaveChanges();

            usuario = await service.GetUsuarioById("usuariotest");

            Assert.Equals(contrase�aEsperada, usuario.Password);
            Assert.Equals(emailEsperado, usuario.Email);
        }
    }
}