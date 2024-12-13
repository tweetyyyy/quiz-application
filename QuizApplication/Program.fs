open System
open System.Windows.Forms
open System.Drawing


// Stopwords and punctuation for text processing
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
    // ziading the tamer = ["greatest", "common"]
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

    similarity >= 0.7 // Threshold: at least 70% match

// Mutable variables for tracking state
let mutable currentQuestionIndex = 0
let mutable userAnswers = []

let createQuizForm () =
    let form = new Form(Text = "F# Quiz", Width = 600, Height = 500)

    // UI Components
    let questionLabel = new Label(Text = "", AutoSize = true, Location = Point(20, 20), MaximumSize = Size(550, 0))
    let optionsPanel = new Panel(Location = Point(20, 60), Width = 550, Height = 200, AutoScroll = true)
    let submitButton = new Button(Text = "Submit", Location = Point(20, 280))
    let resultLabel = new Label(Text = "", AutoSize = true, Location = Point(20, 320), ForeColor = Color.Blue)

    // Show current question
    let showQuestion () =
        let question = questions.[currentQuestionIndex]
        questionLabel.Text <- question.Text
        optionsPanel.Controls.Clear()

        if question.Options.IsEmpty then
            let textBox = new TextBox(Width = 500)
            optionsPanel.Controls.Add(textBox)
        else
            for i, option in List.indexed question.Options do
                let radioButton = new RadioButton(Text = option, AutoSize = true, Location = Point(0, i * 30))
                optionsPanel.Controls.Add(radioButton)

[<EntryPoint>]
let main _ =
    Application.Run(createQuizForm())
    0
