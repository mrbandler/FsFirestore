namespace FsFirestore

/// Contains type definitions that makes it easier to interact with the API.
module Types =
    
    open System
    open System.Reflection
    open Google.Cloud.Firestore

    /// Filters 'FirestoreAttribute' classes from a given sequence.
    let private filterFirestoreAttributePredicate (customAttributes: CustomAttributeData seq) =
        customAttributes
        |> Seq.exists (fun x -> x.AttributeType = typeof<FirestorePropertyAttribute>)

    /// Abstract base class for easier interaction with the API.
    [<AbstractClass>]
    type FirestoreDocument() =
        member val id = "" with get, set 
        member val collectionId = "" with get, set

        member this.allFields =
            this.getFirestoreProperties
            |> Array.map (fun (prop: PropertyInfo) -> (prop.GetValue(this)))

        member this.fields ([<ParamArray>] names: string[])  =  
            this.getFirestoreProperties
            |> Array.filter (fun (prop: PropertyInfo) -> names |> Array.contains prop.Name)
            |> Array.map (fun (prop: PropertyInfo) -> (prop.GetValue(this)))

        member private this.getFirestoreProperties =
            this.GetType().GetProperties()
            |> Array.filter (fun prop -> (not (prop.Name = "id" || prop.Name = "collectionId" || prop.Name = "fields")))
            |> Array.filter (fun prop -> (filterFirestoreAttributePredicate prop.CustomAttributes))            