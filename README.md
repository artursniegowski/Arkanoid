# NEWS_API
News_API is a simple program that makes use of the free online News Api 
(https://newsapi.org/) and based on our search we will retrive the data 
and visualizatize it in an interactive matter with python Pygal package. 
It also includes simple python unittests for error checking. 

Program was developed using Python 3.10.4 , requests, pygal , and a virtual 
environment. Everything is included in the repository (including the virtual 
enviroment ). 

To run the program you have to run the main.py file and adjust the .env.example
file. First change its name, from .env.example -> .env and you have to obtain a
api key from https://newsapi.org/ . After registering you will have your 
individual API_KEY that you need to use in your .env file. 

The file "requirements.txt" includes all the python packages that need to be 
installed to run this game

A file NEWS_HEADLINES.svg (showing all the results in an interactive way) 
will be created in the data folder which is best to open with a webbrowser. 