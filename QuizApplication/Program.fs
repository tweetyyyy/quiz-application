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

    { Id = 10
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

let evaluateAnswer (userAnswer: string) (correctAnswer: string) =
    userAnswer.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase)

let calculateScore (userAnswers: UserAnswer list) (questions: Question list) =
    userAnswers
    |> List.fold (fun acc userAnswer ->
        match questions |> List.tryFind (fun q -> q.Id = userAnswer.QuestionId) with
        | Some question ->
            if question.Options.IsEmpty then
                // Written question
                if evaluateWrittenAnswer userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
            else
                // Multiple-choice question
                if evaluateMultipleChoice userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
        | None -> acc
    ) 0
    
let stopWords = Set.ofList ["the"; "a"; "an"; "and"; "of"; "to"; "in"; "for"; "on"; "at"; "by"; "with"]

let punctuationChars = Set.ofList ['.'; ','; ';'; ':'; '!'; '?'; '\''; '"'; '('; ')'; '['; ']'; '{'; '}'; '-'; '_']


let removePunctuation (text: string) =
    text |> String.filter (fun c -> not (punctuationChars.Contains(c)))


let stem (word: string) =
    if word.EndsWith("ing") then word.Substring(0, word.Length - 3)
    elif word.EndsWith("ed") then word.Substring(0, word.Length - 2)
    elif word.EndsWith("es") || word.EndsWith("s") then word.Substring(0, word.Length - 1)
    else word

let tokenizeAndStem (text: string) =
    text
    |> removePunctuation
    |> fun cleanedText ->
        cleanedText.ToLower().Split([| ' '; '\n'; '\t' |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.filter (fun word -> not (stopWords.Contains(word)))
    |> Array.map stem
    |> Set.ofArray


let isAnswerCorrect (userAnswer: string) (correctAnswer: string) =
    let userTokens = tokenizeAndStem userAnswer
    let correctTokens = tokenizeAndStem correctAnswer

    let intersection = Set.intersect userTokens correctTokens
    let similarity = float intersection.Count / float correctTokens.Count

    similarity >= 0.7 
let calculateScore (userAnswers: UserAnswer list) (questions: Question list) =
    userAnswers
    |> List.fold (fun acc userAnswer ->
        match questions |> List.tryFind (fun q -> q.Id = userAnswer.QuestionId) with
        | Some question ->
            if question.Options.IsEmpty then     
                if isAnswerCorrect userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
            else    
                if evaluateAnswer userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
        | None -> acc
    ) 0



    
