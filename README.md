Coding Challenge - Richard Griffiths
=================================== 


**Answer**

I changed the solution to a Windows Forms presentation, allowing me to embed a graph and display the answers to the original questions in a text box. 

**Highlights**

* Switch to Windows Form
* Embedded graph
* Treatment selector - Top 1 - 5 spenders for any treatment type *See notes about speed*
* Corrections in Solver - utilising LINQ
* CsvHelper implemented in Loader
* Error handling and Using pattern in Loader
* Factory has internal static properties preventing some redundancy in loading
* Some unit tests

*Update*
I cheated slightly by adding a bit of Task threading since I sent the task in. This simply means the form will load before the data has fully finished processing. 

Modifications by section:

**Loader**

The IDatacache public property checks to see if the private field has been populated - returning it if so, otherwise it will load the data for you. This pattern of private fields and public properties checking to see if they are already populated before going to get them is commonly used in the solution.

Here I added an error handling function and refactored the webloading to try and prevent problems if there is no net connection or C:. 

CsvHelper was implemented to allow the CSV to be parsed via a custom mapping to a list of the appropriate objects.

**Solver**

Extensive use of LINQ was employed to refactor and add functionality in this module. The average was corrected - assuming average cost in total per practice - by using the LINQ average function. 

Removed unused float value.

A number of new functions have been added including:

* GetDataByBNFCode - Grouping by treatment type
* Highest_LowestSpendPerBNF - Allows for querying by treatment which practice spends the most and least. Underused in the current form.

**GraphViewer**

I added this module to allow one of the nice to haves to become the main feature, the graph itself. The data can be explored in a slightly more interactive way. You can alternate between Top and bottom spenders, specifying how many you wish to see (try 100, then hit Top or Bottom). The top spenders per treatment type are available to you via the right hand side combo box. This will take a few seconds to load the first time you run it, but once loaded, will run fairly quickly.

**Tests - DataTests.cs**

I invested some time in learning how to use NUnit in order to help investigate some of my LINQ queries and have implemented it here as a demonstration. One test, now however, runs forever - noted in issues.

**Issues**

1. If the app was larger, it would be better as a true layered architecture. Here, I've polluted the View layer with some processing work and even an extension thrown in to simplify dupe key checking on a dictionary.

1. Speed, on the release version, appears only slightly quicker than the original and the memory usage is about 931mb. However, despite checking the implementation of Using via at critical points, I notice it stayed in memory after exiting.

1. Async loading would help with the loading time - at present you may be waiting up to 30 seconds (21 on my pc) for the form to display. In production, I'd want to see progress far sooner.

1. The performance of the CsvHelper may be improved with further investigation - I've used it in a simple way, mapping to a custom class and converting the member types at the same time. This avoided any casting later on, however, this may have impacted the load times.

1. DoesgetHighest_LowestSpendPerBNFReallyWork - this test needs further investigating as during some re factoring it no longer runs, yet the value it produces appears to be working in the program itself. 

1. The interface, ISolver, is looking fat - mixed cohesion as I have queries mixed with data. A rewrite would separate out the contracts more clearly.

2. Naming/Purpose in some Solver functions need some work - revisiting these in the future may be confusing. In addition, the structure of Solver could be tighter.
 
3. I was unable to get rid of some Disposal issues thrown up by the All Rules code analysis I ran - these were in the GraphViewer.Designer.cs file rather than my code.
4. The Fakes folder with Foo and Bar need to be in the current executable directory - IF there is no web connection.

**End of Answer**

Welcome to the coding challenge… 

We have provided a very basic (and often flawed) implementation of an application to load, parse and process NHS health data. We would like you to improve it as you see fit.

The brief for the project is to produce a solution that will send the answers to certain questions (contained within the solution) to a customer on a daily basis. 

You may do anything to the solution that you think improves it (although we will need it to run!) but we will be looking for:

	1.	Performance
		A) How long does the application take to run
		B) How much memory does your application use
		C) How much CPU does your application use
		
	2.	Code Cleanup
		A) How many of the known bugs can you find
		
	3.	Answers
		A) Correct answers
		
	4.	Code Quality
		A) Testing
		B) Team review scores
		
	5.  Documentation
		A) What have you done and why have you done it
		
There's no "right" answer and we appreciate you are all busy people! We would suggest spending between 1/2 a day and 1 day on the problems and documenting anything that you spotted but didn't get a chance to fix!

 


