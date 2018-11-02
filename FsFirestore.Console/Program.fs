open FsFirestore.Console

[<EntryPoint>]
let main argv =
    use mre = new System.Threading.ManualResetEventSlim(false)
    DebuggingConsole.run
    mre.Wait()

    0 // Returning an success code.  