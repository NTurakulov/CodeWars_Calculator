# Calculator

# Task description
https://www.codewars.com/kata/52a78825cdfc2cfc87000005

# Solution overview
1. Main logic is implemented using LINQ Expression mechanics
1. Calculator processes the provided mathematical expression with a single `for` cycle parsing arguments/operations and (re)building the expression tree on the go
1. `MathContext` class represents a part (or a whole in general case) of provided expression and used to process subexpressions in parentheses 
1. `XCalculator` class name is used to avoid conflict with the same namespace
1. Is sake of simplicity `double` type is used, not `decimal`
1. Errors collection added to calculator to make `Calculator` a bit more user-friendly (kind of `useful feature`)
1. Unit tests provided including ones from CodeWars. All tests pass successfully

# Assumptions and expectations
According to task description input string will always be a valid expression, so some edge case checks like `expressions starts with operation sign` are ommitted.