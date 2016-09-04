// Listing 16.1
type Aggregation<'T, 'U> = seq<'T> -> 'U

type Sum = Aggregation<int, int>
type Average = Aggregation<float, float>
type Count<'T> = Aggregation<'T, int>

// Listing 16.2
let sum inputs =
    let mutable accumulator = 0
    for input in inputs do
        accumulator <- accumulator + input
    accumulator

// Now you try #1
let length inputs =
    let mutable accumulator = 0
    for input in inputs do
        accumulator <- accumulator + 1
    accumulator
let lettersInTheAlphabet = [ 'a' .. 'z' ] |> length

// Listing 16.3
do
    let sum inputs =
        Seq.fold
            (fun state input -> state + input)
            0
            inputs
    ()

// Listing 16.4
do
    let sum inputs =
        Seq.fold
            (fun state input ->
                let newState = state + input
                printfn "Current state is %d, input is %d, new state value is %d" state input newState
                newState)
            0
            inputs

    sum [ 1 .. 5 ]
    ()

// Now you try #2
let lengthFold inputs =
    Seq.fold
        (fun state input -> state + 1)
        0
        inputs

let foldAlphabet = [ 'a' .. 'z' ] |> lengthFold

let maxFold inputs =
    Seq.fold
        (fun state input -> if input > state then input else state)
        0
        inputs
let shouldBeTwenty = [ 1;2;5;3;20;13;18 ] |> maxFold

// Listing 16.5
let inputs = [ 1 .. 5 ]
Seq.fold (fun state input -> state + input) 0 inputs
inputs |> Seq.fold (fun state input -> state + input) 0
(0, inputs) ||> Seq.fold (fun state input -> state + input)

// Listing 16.6
open System.IO
let mutable totalChars = 0
let sr = new StreamReader(File.OpenRead "book.txt")

while (not sr.EndOfStream) do
    let line = sr.ReadLine()
    totalChars <- totalChars + line.ToCharArray().Length

// Listing 16.7
let lines : string seq =
    seq {
        use sr = new StreamReader(File.OpenRead @"book.txt")
        while (not sr.EndOfStream) do
            yield sr.ReadLine() }

(0, lines) ||> Seq.fold(fun total line -> total + line.Length)

// Listing 16.8
open System
type Rule = string -> bool * string 

let rules : Rule list =
    [ fun text -> (text.Split ' ').Length = 3, "Must be three words"
      fun text -> text.Length <= 30, "Max length is 30 characters"
      fun text -> text.ToCharArray()
                  |> Array.filter Char.IsLetter
                  |> Array.forall Char.IsUpper, "All letters must be caps" ]

// Listing 16.9
let validateManual (rules: Rule list) word =
    let passed, error = rules.[0] word
    if not passed then false, error
    else
        let passed, error = rules.[1] word
        if not passed then false, error
        else
            let passed, error = rules.[2] word
            if not passed then false, error
            else true, ""

// Listing 16.10
let buildValidator (rules : Rule list) =
    rules
    |> List.reduce(fun firstRule secondRule word ->
        let passed, error = firstRule word
        if passed then
            let passed, error = secondRule word
            if passed then true, "" else false, error
        else false, error)

let validate = buildValidator rules
let word = "HELLO FrOM F#"

validate word
 
// Now you try #3
module Rules =
    let threeWordRule (text:string) =
        printfn "Running three word rule"
        (text.Split ' ').Length = 3, "Must be three words"
    let maxLengthRule (text:string) =
        printfn "Running max length rule"
        text.Length <= 30, "Max length is 30 characters"
    let allCapsRule (text:string) =
        printfn "Running all caps rule"
        text.ToCharArray()
        |> Array.filter Char.IsLetter
        |> Array.forall Char.IsUpper, "All letters must be caps"
    let noNumbersRule (text:string) =
        printfn "Running no numbers rule"
        text.ToCharArray()
        |> Array.forall (Char.IsNumber >> not), "Numbers are not permitted"
    
    let allRules = [ threeWordRule; allCapsRule; maxLengthRule; noNumbersRule ]

let debugValidate = buildValidator Rules.allRules
let pass = debugValidate "HELLO FROM F#"
let fail = debugValidate "HELLO FR0M F#"