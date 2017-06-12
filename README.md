CSharp Lua Scripting Engine is a engine designed for creating/editing Lua scripts during CSharp runtime execution and allowing those changes to be present in the execution for debugging purposes.

This engine works by storing all the scripts and allowing objects to create a script each with its own key.

The engine will allow you to reload scripts (copy them from the solution diretory to the output directory) so that any changes made to any Lua scripts will be present during the CSharp execution.