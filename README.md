CSharp Lua Scripting Engine is a engine designed for creating/editing Lua scripts during CSharp runtime execution and allowing those changes to be present in the execution. 

This engine works by separating Lua scripts into two types, Properties and Actions.
- Properties contain a table of all the data the script will use
- Actions contains the functions that all take in a table as a paramter

The engine will allow you to reload either property scripts and/or action scripts so that any changes made to any Lua scripts will be present during the CSharp execution.