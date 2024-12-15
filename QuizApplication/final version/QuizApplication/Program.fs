open System
open System.Windows.Forms
open System.Drawing

type Question = {
    Id: int
    Text: string
    Options: string list
    CorrectAnswer: string
}

let questions = [
    { Id = 1; Text = "Which of the following is a characteristic of functional programming?"; Options = ["Mutable state"; "Side effects"; "Pure functions"; "Sequential execution"]; CorrectAnswer = "Pure functions" };
    { Id = 2; Text = "What is referential transparency?"; Options = []; CorrectAnswer = "A property where an expression can be replaced with its value without changing the program's behavior" };
    { Id = 3; Text = "What is immutability in functional programming?"; Options = ["Data cannot be changed after creation"; "All variables are global"; "Functions depend on external state"; "None of the above"]; CorrectAnswer = "Data cannot be changed after creation" };
    { Id = 4; Text = "What is the output of the following F# expression: suml [1; 2; 3]?"; Options = ["3"; "6"; "1"; "0"]; CorrectAnswer = "6" };
    { Id = 5; Text = "In F#, which operator is used for string concatenation?"; Options = ["&"; "+"; "^"; "*"]; CorrectAnswer = "+" };
    { Id = 6; Text = "What is the result of the expression not (true || false) in F#?"; Options = ["true"; "false"; "Error"; "None"]; CorrectAnswer = "false" };
    { Id = 7; Text = "Which of the following is NOT a basic type in F#?"; Options = ["int"; "float"; "string"; "list"]; CorrectAnswer = "list" };
    { Id = 8; Text = "What type of data structure is used to represent a pair of values in F#?"; Options = ["List"; "Tuple"; "Array"; "Dictionary"]; CorrectAnswer = "Tuple" };
    { Id = 9; Text = "What is the output of the expression String.length \"Hello\"?"; Options = ["4"; "5"; "6"; "Error"]; CorrectAnswer = "5" };
    { Id = 10; Text = "What does the gcd function calculate?"; Options = []; CorrectAnswer = "The greatest common divisor of two integers" };
    { Id = 11; Text = "What is the purpose of pattern matching in F#?"; Options = []; CorrectAnswer = "To execute code based on the structure of data." };
    { Id = 12; Text = "Name one advantage of functional programming."; Options = []; CorrectAnswer = "Easier reasoning about code" };
    { Id = 13; Text = "What is one drawback of functional programming?"; Options = []; CorrectAnswer = "Combining pure functions into a complete application can be challenging" }
]


let stopWords = Set.ofList ["the"; "a"; "an"; "and"; "of"; "to"; "in"; "for"; "on"; "at"; "by"; "with"]
let punctuationChars = Set.ofList ['.'; ','; ';'; ':'; '!'; '?'; '\''; '"'; '('; ')'; '['; ']'; '{'; '}'; '-'; '_']

let removePunctuation (text: string) =
    text |> String.filter (fun c -> not (punctuationChars.Contains(c)))

let stem (word: string) =
    if word.EndsWith("ing") then word.Substring(0, word.Length - 3)
    elif word.EndsWith("est") then word.Substring(0, word.Length - 3)
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

let mutable currentQuestionIndex = 0
let mutable userAnswers = []

let createQuizForm () =
    let form = 
        new Form(Text = "Quiz Application", Width = 700, Height = 550, 
                 BackColor = Color.FromArgb(240, 240, 240), 
                 StartPosition = FormStartPosition.CenterScreen,
                 Font = new Font("Arial", 10.0F))

    let headerLabel = 
        new Label(Text = "Welcome to the F# Quiz!", AutoSize = true, 
                  Location = Point(20, 20), 
                  Font = new Font("Arial", 30.0F, FontStyle.Bold), 
                  ForeColor = Color.FromArgb(126, 109, 243)) 

    let questionLabel = 
        new Label(Text = "", AutoSize = true, Location = Point(20, 100), 
                  MaximumSize = Size(650, 0), 
                  Font = new Font("Arial", 12.0F), 
                  ForeColor = Color.FromArgb(124, 88, 154)) 

    let optionsPanel = 
        new Panel(Location = Point(20, 150), Width = 650, Height = 200, 
                  AutoScroll = true, 
                  BackColor = Color.White, 
                  BorderStyle = BorderStyle.FixedSingle)

    let submitButton = 
        new Button(Text = "Submit", Location = Point(275, 380), 
                   BackColor = Color.FromArgb(126, 109, 243), 
                   ForeColor = Color.White, 
                   Font = new Font("Arial", 10.0F, FontStyle.Bold), 
                   FlatStyle = FlatStyle.Flat, 
                   Width = 120, Height = 40)

    let resultLabel = 
        new Label(Text = "", AutoSize = true, Location = Point(220, 440), 
                  Font = new Font("Arial", 12.0F, FontStyle.Bold), 
                  ForeColor = Color.FromArgb(0, 150, 136)) 

    submitButton.MouseEnter.Add (fun _ -> submitButton.BackColor <- Color.FromArgb(124, 88, 154)) 
    submitButton.MouseLeave.Add (fun _ -> submitButton.BackColor <- Color.FromArgb(126, 109, 243)) 

    let showQuestion () =
        let question = questions.[currentQuestionIndex]
        questionLabel.Text <- question.Text
        optionsPanel.Controls.Clear()

        if question.Options.IsEmpty then
            let textBox = 
                new TextBox(Width = 647,Height = 197, Font = new Font("Arial", 10.0F), 
                            ForeColor = Color.FromArgb(33, 33, 33), BackColor = Color.FromArgb(245, 245, 245), Multiline = true)
            optionsPanel.Controls.Add(textBox)
        else
            for i, option in List.indexed question.Options do
                let yOffset = if i = 0 then 25 else (i * 40) + 25
                let radioButton = 
                    new RadioButton(Text = option, AutoSize = true, Location = Point(10,yOffset) , 
                                    Font = new Font("Arial", 10.0F), 
                                    ForeColor = Color.FromArgb(62, 57, 76)) 
                radioButton.MouseEnter.Add (fun _ -> radioButton.ForeColor <- Color.FromArgb(83, 79, 144)) 
                radioButton.MouseLeave.Add (fun _ -> radioButton.ForeColor <- Color.FromArgb(62, 57, 76)) 
                optionsPanel.Controls.Add(radioButton)

    let handleSubmission _ =
        let question = questions.[currentQuestionIndex]
        let userAnswer =
            if question.Options.IsEmpty then
                (optionsPanel.Controls.[0] :?> TextBox).Text
            else
                optionsPanel.Controls
                |> Seq.cast<RadioButton>
                |> Seq.tryFind (fun rb -> rb.Checked)
                |> Option.map (fun rb -> rb.Text)
                |> Option.defaultValue ""

        userAnswers <- (question.Id, userAnswer) :: userAnswers
        currentQuestionIndex <- currentQuestionIndex + 1

        if currentQuestionIndex < questions.Length then
            showQuestion()
        else
            let score =
                userAnswers
                |> List.fold (fun acc (qid, answer) ->
                    let question = questions |> List.find (fun q -> q.Id = qid)
                    if question.Options.IsEmpty then
                        if isAnswerCorrect answer question.CorrectAnswer then acc + 1 else acc
                    else
                        if answer = question.CorrectAnswer then acc + 1 else acc
                ) 0
            resultLabel.Text <- sprintf "Quiz finished! Your score: %d/%d" score questions.Length
            resultLabel.ForeColor <- if score >= questions.Length / 2 then Color.FromArgb(76, 175, 80) 
                                     else Color.FromArgb(244, 67, 54) 
            submitButton.Enabled <- false

    submitButton.Click.Add handleSubmission

    form.Controls.Add(headerLabel)
    form.Controls.Add(questionLabel)
    form.Controls.Add(optionsPanel)
    form.Controls.Add(submitButton)
    form.Controls.Add(resultLabel)

    showQuestion()
    form


[<EntryPoint>]
let main _ =
    Application.Run(createQuizForm())
    0
