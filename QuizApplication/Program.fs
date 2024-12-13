module Program.fs
open System 


let evaluateAnswer (userAnswer: string) (correctAnswer: string) =
    userAnswer.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase)

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

let askQuestions (questions: Question list) =
    let mutable userAnswers = []

    for question in questions do
        printfn "\nQuestion %d: %s" question.Id question.Text

        match question.Options with
        | [] -> 
            printf "Your answer: "
            let answer = Console.ReadLine().Trim().ToLower()
            userAnswers <- { QuestionId = question.Id; Answer = answer } :: userAnswers
        | options ->
            for i, option in List.indexed options do
                printfn "%d. %s" (i + 1) option
            printf "Your answer (choose an option number): "

            let mutable validAnswer = false
            while not validAnswer do
                match Int32.TryParse(Console.ReadLine()) with
                | (true, index) when index > 0 && index <= options.Length ->
                    let selectedOption = options.[index - 1]
                    userAnswers <- { QuestionId = question.Id; Answer = selectedOption } :: userAnswers
                    validAnswer <- true
                | _ ->
                    printf "Invalid input. Please enter a number between 1 and %d: " options.Length

    userAnswers

let runQuiz () =
    let userAnswers = askQuestions questions
    let score = calculateScore userAnswers questions
    printfn "\nQuiz finished! Your final score is %d out of %d." score questions.Length

runQuiz ()
