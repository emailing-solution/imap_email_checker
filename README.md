# imap email checker

This is a simple C# application for checking email logins using the IMAP protocol. It allows you to test a list of email and password combinations against a specific IMAP server. The application supports multi-threading for faster checking and also allows the use of SOCKS proxies for anonymity.

## Usage

1. Clone or download this repository to your local machine.
2. Build the project using your preferred C# development environment (e.g., Visual Studio).
3. Prepare your email and password combinations in a text file named `combo.txt` where each line follows the format: `email:password`.
4. If you want to use SOCKS proxies, create a text file named `proxies.txt` with the format `proxy_ip:proxy_port:proxy_username:proxy_password`. If you don't need proxies, you can skip this step.
5. Run the compiled application.

### Configuration

When you run the application, you will be prompted to provide the following information:

- **Thread Amount**: Enter the number of threads you want to use for parallel checking. More threads can increase the speed of the checking process.
- **IMAP Domain**: Enter the IMAP server domain you want to check against (e.g., "imap.example.com").
- **IMAP Port**: Enter the IMAP server port (default is 993 for SSL connections).
- **Use SSL (yes/no)**: Decide whether you want to use SSL encryption for the IMAP connections. Type "yes" for SSL or "no" for non-SSL connections.

### Output

The application will start checking the email and password combinations and display the results in the console:

- `[HIT]` indicates a successful login.
- `[MISS]` indicates a failed login.
- `[FAIL]` indicates an issue during the connection or authentication process.

The results are also written to a `hits.txt` file for successful logins.

## Dependencies

This application relies on the Chilkat library for IMAP operations. Make sure to include the Chilkat library in your project or application.

## Author

This application was created by Malohtie (MED AMINE EL ATTABI). You can contact the author on Telegram: [@malohtie](https://t.me/malohtie).

>[!NOTE]
>Please use this application responsibly and only for legitimate purposes. Unauthorized use for malicious activities is strictly prohibited.

![image](https://github.com/emailing-solution/imap_email_checker/assets/20033279/2e7227d8-93cf-442b-83a5-d652fbc9ab50)

