using FirebaseLogin.Network;

namespace FirebaseLogin {
    public partial class Login : Form {
        private FirebaseService authService;

        public Login()
        {
            InitializeComponent();
            // Crie um HttpClient e passe-o para o FirebaseService
            var httpClient = new HttpClient();
            authService = new FirebaseService(httpClient);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            try
            {
                var token = await authService.LoginWithEmailAndPassword(email, password);
                MessageBox.Show("Login efetuado com sucesso! Token: " + token);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao fazer login: " + ex.Message);
            }
        }
    }
}
