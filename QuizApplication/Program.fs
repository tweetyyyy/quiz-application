module Program.fs
open System 

type Question = {
    Id: int
    Text: string
    Options: string list // Empty for written questions
    CorrectAnswer: string
}

type UserAnswer = {
    QuestionId: int
    Answer: string
}

let questions = [
    { Id = 1
      Text = "Which of the following is a characteristic of functional programming?"
      Options = ["Mutable state"; "Side effects"; "Pure functions"; "Sequential execution"]
      CorrectAnswer = "Pure functions" }

    { Id = 2
      Text = "What is referential transparency?"
      Options = [] // Written question
      CorrectAnswer = "A property where an expression can be replaced with its value without changing the program's behavior" }

    { Id = 3
      Text = "In F#, which operator denotes functional composition?"
      Options = ["<<"; "+"; "*"; "/"]
      CorrectAnswer = "<<" }
      
    { Id = 4
      Text = "What is the output of the following F# expression: suml [1; 2; 3]?"
      Options = ["3"; "6"; "1"; "0"]
      CorrectAnswer = "6" }

    { Id = 5
      Text = "In F#, which operator is used for string concatenation?"
      Options = ["&"; "+"; "^"; "*"]
      CorrectAnswer = "+" }

    { Id = 6
      Text = "What is the result of the expression not (true || false) in F#?"
      Options = ["true"; "false"; "Error"; "None"]
      CorrectAnswer = "false" }

    { Id = 7
      Text = "Which of the following is NOT a basic type in F#?"
      Options = ["int"; "float"; "string"; "list"]
      CorrectAnswer = "list" }

    { Id = 8
      Text = "What type of data structure is used to represent a pair of values in F#?"
      Options = ["List"; "Tuple"; "Array"; "Dictionary"]
      CorrectAnswer = "Tuple" }

    { Id = 9
      Text = "What is the output of the expression String.length \"Hello\"?"
      Options = ["4"; "5"; "6"; "Error"]
      CorrectAnswer = "5" }

    { Id = 2
      Text = "What does the gcd function calculate?"
      Options = [] // Written question
      CorrectAnswer = "The greatest common divisor of the a an and to in for on at by with two integers" }

    { Id = 11
      Text = "What is the purpose of pattern matching in F#?"
      Options = [] // Written question
      CorrectAnswer = "To execute code based on the structure of data." }

    { Id = 12
      Text = "Name one advantage of functional programming."
      Options = [] // Written question
      CorrectAnswer = "Easier reasoning about code" }

    { Id = 13
      Text = "What is one drawback of functional programming?"
      Options = [] // Written question
      CorrectAnswer = "Combining pure functions into a complete application can be challenging" }
]
