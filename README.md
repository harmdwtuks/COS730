# COS730
COS730 Assignment

<h3>Introcucing myself</h3>

Hi, I am Harm de Wet (u20798441) a part-time Hons. Computer Science student at the University of Pretoria. I am furthuring my studies to acieve my goal of being a data scientist and software engineer knowing multiple sides of the industry with the hopes to own my own Software Development House.

I currently work as a senior developer for a digital forensics and analytics company working on the core products of the company. I am given a lot of free roam at the compay to advance the systems in any sensible manner I deem fit.

The art of digital forensics was one of the most fascinating fields until I discovered data science and software engineering, now I am in 3 minds and need to find a way to combine all of these sub-fields into one.

My main interest is software development (mainly the back-end), neural networks, videography and photography in my spare time.

My main skill set includes (but not limited to):
  <ul>
  <li>Software development 
    <ul>
      <li>ASP.NET (Full Stack)</li>
      <li>C++</li>
      <li>SQL (MSSQL)</li>
      <li>Neural networks</li>
      </ul>
    </li>
  <li>Software project management</li>
  </ul>

Please find my LinkedIn profile here: https://www.linkedin.com/in/harm-de-wet-031ba0a6

<h3>Link to User Manual for <i>CoachIt</i></h3>

https://drive.google.com/file/d/1c9N2OUJtBMIZPcofn-PmDefJpH9k8BYe/view?usp=sharing

<h3>Links to Project Management Documents Used For the Development of <i>CoachIt</i></h3>

https://drive.google.com/file/d/13u_QAVsh2v-Gkp_ItXFeODpcpq5PJhwj/view?usp=sharing

https://github.com/harmdwtuks/COS730/projects/1

<h3>Steps to Deploy <i>CoachIt</i></h3>

After the code has been downloaded. Create a release build of each of the projects.

Copy the files to an IIS hosting environment with each project's release in its own location in the environment.

Confugure only the Interaction layer with IIS to be externally accessible (this is the only one that users will connect to directly.)

The other projects can run each on a port that you set in the IIS settings.

Update the Web.Config of each project in IIS with the new URLs of each of the other components of the system.

For a true microservice environment, all projects with 'MS' at the end should be configured to run in their own containers.

Setup SQL Server on the machine with a database user dedicated to only the CoachIt database that you need to create using the following script in this repository:

COS730\DatabaseLayer\Database.sql updated with you environment details and updating the default user to be you.

Update the Web.config file of the database layer with the new database connection string (with the detials of the instance that you just created)

To gain access as the first user, open the application (interaction layer/front end) using a web-browser and performing a forgot password action to receive a link to reset your password. (The email's cridentials used to send the email will have to be specified in the UserManagerMS Web.Config file - defaults for the account has not been created yet.)

Now log in and enjoy using the system!

<h3>Steps to Develop <i>CoachIt</i></h3>

Download the code and setup in Visual studio.

In the solution fonfiguration, make sure all of the apps are set to start up together (you need them all to use the system).

Update the Web.Config of each project with the new URLs of each of the other components of the system.

Setup SQL Server on the machine with a database user dedicated to only the CoachIt database that you need to create using the following script in this repository:

COS730\DatabaseLayer\Database.sql updated with your environment details and updating the default user to be you.

Update the Web.config file of the database layer with the new database connection string (with the details of the instance that you just created)

To gain access as the first user, open the application (interaction layer/front end) using a web-browser and performing a forgot password action to receive a link to reset your password. (The email's cridentials used to send the email will have to be specified in the UserManagerMS Web.Config file - defaults for the account has not been created yet.)

Now you're ready to log in for testing and setup to develop!

