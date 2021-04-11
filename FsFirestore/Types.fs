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
    [<AllowNullLiteral>]
    [<AbstractClass>]
    type FirestoreDocument() =
        member val Id = "" with get, set 
        member val CollectionId = "" with get, set
    
        /// Returns all FirestoreProperty properties of this instance.
        member private this.GetFirestoreProperties =
            this.GetType().GetProperties()
            |> Array.filter (fun prop -> (not (prop.Name = "id" || prop.Name = "collectionId" || prop.Name = "fields")))
            |> Array.filter (fun prop -> (filterFirestoreAttributePredicate prop.CustomAttributes)) 

        /// Returns all FirestoreProperty fields in a object array.
        member this.AllFields =
            this.GetFirestoreProperties
            |> Array.map (fun (prop: PropertyInfo) -> (prop.GetValue(this)))

        /// Returns specified FirestoreProperty fields in a object arrray.
        member this.Fields ([<ParamArray>] names: string[])  =  
            this.GetFirestoreProperties
            |> Array.filter (fun (prop: PropertyInfo) -> names |> Array.contains prop.Name)
            |> Array.map (fun (prop: PropertyInfo) -> (prop.GetValue(this)))           

    /// Generic query change type.
    type DocumentChange<'T when 'T : not struct> = {
        document: 'T
        changeType: DocumentChange.Type
        newIndex: int option
        oldIndex: int option
    }