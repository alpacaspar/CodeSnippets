Sea Monster Boss AI created for Into the Deep (https://ploopploop.itch.io/into-the-deep)

See the AI in action in this video: https://youtu.be/6oTJ1g-LONM?si=K602b1IqtQM1Qi7t

The Boss script (Boss.cs) uses a queue, allowing a designer to create an attack pattern by populating the list with actions that the boss can take.
Included in the CodeSnippets are two examples of actions the boss can take, like the bite (BiteAction.cs) and the swim (SwimAction.cs) actions.
These actions all inherit from the Action class (Action.cs).

The boss itself uses a simple state machine (FiniteStateMachine.cs) to control it's behaviour, which is implemented in the Boss class. 