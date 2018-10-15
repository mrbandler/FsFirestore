namespace FsFirestore

module Env =
    
    open System
    open Microsoft.FSharpLu.File
    open FSharp.Json

    [<Literal>] 
    let private GCP_SYSTEM_VAR = "GOOGLE_APPLICATION_CREDENTIALS"

    /// GCP Credentials.
    type Credentials = {
        //t: string
        project_id: string
        private_key_id: string
        private_key: string
        client_email: string
        client_id: string
        auth_uri: string
        token_uri: string
        auth_provider_x509_cert_url: string
        client_x509_cert_url: string
    }

    /// Deserializes the GCP credentials found by the given path.
    let private deserializeGCPCredentials path = 
        Json.deserialize<Credentials> (atomaticReadAllText path)

    /// Sets the GCP environment for the current process.
    /// If the setup is successfull the firebase project ID will be returned.
    let internal setupGCPEnvironment path =
        match getExistingFile path with
        | Some path -> 
            Environment.SetEnvironmentVariable(GCP_SYSTEM_VAR, path)
            Some((deserializeGCPCredentials path).project_id)
        | None -> None
        