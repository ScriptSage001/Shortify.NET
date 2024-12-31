<h1 align="center" id="title">Shortify.NET</h1>

<p align="center"><img src="https://github.com/ScriptSage001/Shortify.NET/blob/master/Assets/Images/Logo.png?raw=true" alt="logo"></p>

<p align="center"><img src="https://socialify.git.ci/ScriptSage001/Shortify.NET/image?description=1&amp;descriptionEditable=A%20powerful%20.NET%208%20URL%20shortener%20with%20JWT%20auth%2C%20analytics%2C%20and%20caching%2C%20designed%20for%20scalability%20and%20security.&amp;font=Raleway&amp;language=1&amp;name=1&amp;owner=1&amp;pattern=Plus&amp;theme=Dark" alt="project-image"></p>

<p id="description">A powerful .NET 8 URL shortener with JWT auth analytics and caching designed for scalability and security.</p>

<br>

<h3>Code Quality</h3>

[![Qodana](https://github.com/ScriptSage001/Shortify.NET/actions/workflows/qodana_code_quality.yml/badge.svg)](https://github.com/ScriptSage001/Shortify.NET/actions/workflows/qodana_code_quality.yml)

<br>

<h2>✨ Why Use Shortify.NET?</h2>

- **Scalable and Secure:** Built with .NET 8, ensuring high performance and strong security measures.
- **JWT Authentication:** Secure user management and role-based access control.
- **Caching:** Accelerated redirection using Redis for optimal performance.
- **Ease of Use:** A simple API interface, designed to integrate seamlessly into any application.
- **Analytics:** Gain insights into URL usage patterns. (Coming Soon)

<br>

<h2>🧩 Features</h2>

- Generate short URLs quickly and efficiently.
- Secure endpoints with JWT authentication.
- Track detailed analytics for shortened URLs.
- Support for custom aliases.
- Scalable design for high availability.
- Dockerized for easy deployment.

<br>

<h2>🚀 Tech Stack</h2>

- **Backend:** .NET 8
- **Database:** PostgreSQL (Supabase-hosted)
- **Caching:** Redis
- **Containerization:** Docker
- **API Documentation:** Swagger
- **CI/CD:** GitHub Actions
- **Hosting Platform:** Render
- **Static Code Analysis:** Qodana

<br>

<h2>🛠️ Installation and Setup:</h2>
<h3>Using Docker</h3>

<p>1. Pull the Docker Image:</p>

```
docker pull thescriptsage/shortifynetapi
```
<br>

<p>2. Set Up Required Environment Variables:</p>
Ensure the following environment variables are set before running the container:

- **DB_CONNECTION:** Connection string for the PostgreSQL database (e.g., Host=localhost;Port=5432;Database=Shortify;Username=yourUsername;Password=yourPassword).
- **REDIS_CONNECTION:** Connection string for Redis (e.g., localhost:6379).
- **APP_SECRET:** A secret key for signing JWT tokens.
- **CLIENT_SECRET:** Client-specific secret for enhanced security.
- **SENDER_EMAIL:** Email address for sending OTPs or notifications.
- **SENDER_EMAIL_PASSWORD:** Password for the sender email.
- **ALLOWED_HOST:** A comma-separated list of allowed host URLs.

<br>

<p>3. Run the Docker Container:</p>

```
docker run -d -p 5000:80 \
  -e DB_CONNECTION="Host=localhost;Port=5432;Database=Shortify;Username=yourUsername;Password=yourPassword" \
  -e REDIS_CONNECTION="localhost:6379" \
  -e APP_SECRET="yourAppSecret" \
  -e CLIENT_SECRET="yourClientSecret" \
  -e SENDER_EMAIL="yourEmail@gmail.com" \
  -e SENDER_EMAIL_PASSWORD="yourEmailPassword" \
  -e ALLOWED_HOST="http://localhost,http://example.com" \
  thescriptsage/shortifynetapi
```
<br>

<p>3. Access the Swagger UI:</p>
Visit <a target="_blank" href="http://localhost:5000/swagger/index.html">http://localhost:5000/swagger/index.html</a> to explore the API.

<br><br>
<h3>Local Development</h3>

<p>1. Clone the Repository:</p>

```
git clone https://github.com/ScriptSage001/Shortify.NET.git
cd Shortify.NET
```
<br>

<p>2. Configure Environment Variables:</p>
Use an environment variable manager or .env file to configure the following values:

- **DB_CONNECTION:** Connection string for the PostgreSQL database (e.g., Host=localhost;Port=5432;Database=Shortify;Username=yourUsername;Password=yourPassword).
- **REDIS_CONNECTION:** Connection string for Redis (e.g., localhost:6379).
- **APP_SECRET:** A secret key for signing JWT tokens.
- **CLIENT_SECRET:** Client-specific secret for enhanced security.
- **SENDER_EMAIL:** Email address for sending OTPs or notifications.
- **SENDER_EMAIL_PASSWORD:** Password for the sender email.
- **ALLOWED_HOST:** A comma-separated list of allowed host URLs.
<br>
<h4>Example .env file:</h4>

```
DB_CONNECTION=Host=localhost;Port=5432;Database=Shortify;Username=yourUsername;Password=yourPassword
REDIS_CONNECTION=localhost:6379
APP_SECRET=yourAppSecret
CLIENT_SECRET=yourClientSecret
SENDER_EMAIL=yourEmail@gmail.com
SENDER_EMAIL_PASSWORD=yourEmailPassword
ALLOWED_HOST=http://localhost,http://example.com
```
<br>

<p>3. Install Dependencies:</p>
Ensure you have the .NET 8 SDK installed. Then, restore the NuGet packages:

```
dotnet restore
```
<br>

<p>4. Run the Application:</p>

```
dotnet run
```
<br>

<p>5. Access the Swagger UI:</p>
<p>Swagger UI will be available at <a target="_blank" href="http://localhost:5000/swagger/index.html">http://localhost:5000/swagger/index.html</a> or the port specified in the console logs.</p>

<br>

<h2>📚 API Documentation</h2>
The API is fully documented using Swagger. Access the live documentation here:

- <a target="_blank" href="https://shortify-net.onrender.com/swagger/index.html">Swagger UI</a>
- <a target="_blank" href="https://shortify-net.onrender.com/swagger/v1/swagger.json">Swagger JSON</a>

<br>

<h2>🍰 Contribution Guidelines:</h2>

<p>Contributions are what make the open source community such an amazing place to learn inspire and create. Any contributions you make are greatly appreciated.</p> 
<p>If you have a suggestion that would make this better please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement". Don't forget to give the project a star! Thanks again!</p> 

<h3>How to Contribute</h3> 
<p>1. Fork the Project </p>
<p>2. Create your Feature Branch</p> 

``` 
git checkout -b feature/AmazingFeature
```
<br>

<p>3. Commit your Changes</p> 

``` 
git commit -m 'Add some AmazingFeature'
```
<br>

<p>4. Push to the Branch</p> 

``` 
git push origin feature/AmazingFeature
```
<br>

<p>5. Open a Pull Request</p>

<br>

<h2>📸 Screenshots</h2>

<h3>Swagger UI</h3>
<p align="center"><img src="https://github.com/ScriptSage001/Shortify.NET/blob/master/Assets/Images/Swagger%20UI%200.png?raw=true" alt="swagger-ui-01"></p>
<p align="center"><img src="https://github.com/ScriptSage001/Shortify.NET/blob/master/Assets/Images/Swagger%20UI%201.png?raw=true" alt="swagger-ui-02"></p>
<p align="center"><img src="https://github.com/ScriptSage001/Shortify.NET/blob/master/Assets/Images/Swagger%20UI%202.png?raw=true" alt="swagger-ui-03"></p>

<br>

<h2>🛡️ License:</h2>
<p>This project is licensed under the Apache License. See the <a href="https://github.com/ScriptSage001/Shortify.NET/blob/master/LICENSE.txt">LICENSE</a> file for details.</p>

<br>

<h2>🌟 Acknowledgments</h2>

- Inspiration from modern URL shorteners like Bitly.
- Thanks to the .NET community for continuous support and tools.
