module Capstone3.Program

open System
open Capstone3.Domain
open Capstone3.Operations

let withdrawWithAudit = auditAs "withdraw" Auditing.composedLogger withdraw
let depositWithAudit = auditAs "deposit" Auditing.composedLogger deposit

[<AutoOpen>]
module Commands =
    let accountCommands = 
        [ 'd', depositWithAudit
          'w', withdrawWithAudit
          'x', fun _ account -> account ]
        |> Map.ofList
    let isValidCommand = accountCommands.ContainsKey
    let isStopCommand = (=) 'x'

[<EntryPoint>]
let main _ =
    let account =
        let loadAccountFromDisk = FileRepository.findTransactionsOnDisk >> Operations.loadAccount
        let name =
            Console.Write "Please enter your name: "
            Console.ReadLine()
        loadAccountFromDisk name
    printfn "Current balance is £%M" account.Balance

    let commands = seq {
        while true do
            Console.Write "(d)eposit, (w)ithdraw or e(x)it: "
            yield Console.ReadKey().KeyChar }
    
    let getAmount command =
        Console.WriteLine()
        Console.Write "Enter Amount: "
        command, Console.ReadLine() |> Decimal.Parse

    let processCommand account (command, amount) =
        Console.Clear()
        let account = account |> accountCommands.[command] amount
        printfn "Current balance is £%M" account.Balance
        account

    let account =
        commands
        |> Seq.filter isValidCommand
        |> Seq.takeWhile (not << isStopCommand)
        |> Seq.map getAmount
        |> Seq.fold processCommand account
    
    Console.Clear()
    printfn "Closing Balance:\r\n %A" account
    Console.ReadKey() |> ignore

    0