// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    let readLines filePath = System.IO.File.ReadLines(filePath)
    
    //readLines(argv);
    
    0 // return an integer exit code

