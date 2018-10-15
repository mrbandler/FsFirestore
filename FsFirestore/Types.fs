namespace FsFirestore

/// Contains type definitions that makes it easier to interact with the API.
module Types =
    
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

        member this.fields =
            this.GetType().GetProperties()
            |> Array.filter (fun prop -> (not (prop.Name = "id" || prop.Name = "collectionId" || prop.Name = "fields")))
            |> Array.filter (fun prop -> (filterFirestoreAttributePredicate prop.CustomAttributes))
            |> Array.map (fun prop -> (prop.GetValue(this)))