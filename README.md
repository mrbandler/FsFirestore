<img src="https://raw.githubusercontent.com/mrbandler/FsFirestore/master/Icons/FsFirestoreTransparentBanner.png" alt="FsFirestore Icon Banner" width="150" height="110"/>

# FsFirestore 

[![pipeline status](https://gitlab.com/mrbandler/FsFirestore/badges/master/pipeline.svg)](https://gitlab.com/mrbandler/FsFirestore/commits/master) [![NuGet Badge](https://buildstats.info/nuget/FsFirestore?includePreReleases=true)](https://www.nuget.org/packages/FsFirestore)

**Functional F# library to access Firestore database hosted on Google Cloud Platform (GCP) or Firebase.**

## Table Of Content
1. [Usage](#1-usage) ⌨️
2. [Bugs and Features](#2-bugs-and-features) 🐞💡
3. [Buy me a coffee](#3-buy-me-a-coffee) ☕
4. [License](#4-license) 📃

---

## 1. Usage

### Connect to Firestore

To use any of the Firestore features you have to initialize the connection via a Service Account JSON (either for [Firebase](https://console.firebase.google.com/project/_/settings/serviceaccounts/adminsdk) or [GCP](https://cloud.google.com/docs/authentication/getting-started)).

```fsharp
open FsFirestore.Firestore

let didConnect = connectToFirestore "./path/to/your/service_account.json"
```

The `connectToFirestore` function returns a boolean, to indicate whether the connection could be established.

### Create, Read, Update and Delete (CRUD)

After your successfully connected to your Firestore you can start manipulating data.

#### Model Classes

For better handling within the API we added generic retrieval functions, to simply retrieve your wanted model. Sadly the Google .NET API only let's us use classes, to compensate we created a base class that can be used to make your life easier. For a detailed description on data models for Firebase read [this](http://googleapis.github.io/google-cloud-dotnet/docs/Google.Cloud.Firestore/datamodel.html).

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Types

[<FirestoreData>]
type Address() =
	inherit FirestoreDocument() // Base class that comes with FsFirestore
	
	[<FirestoreProperty>]
	member val Street = "Pennsylvania Avenue" with get, set

	[<FirestoreProperty>]
	member val HouseNo = 1600 with get, set
	
	[<FirestoreProperty>]
	member val City = "Washington" with get, set
	
	[<FirestoreProperty>]
	member val State = "DC" with get, set

// Because of the inheritance we now have some niffty features in this straight forward model class.
let address = new Address()

// We can retrieve all fields in a object list, 
// which can later be used to query Firestore.
let fields = address.AllFields // => ["Pennsylvania Avenue"; 1600; "Washington"; "DC"]
let specificField = address.Fields("HouseNo", "City") // => [1600; "Washington"]

// We can ask the model which ID and collection it belongs to.
let docId = address.Id
let collectionId = address.CollectionId
```

#### Reading Documents

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Firestore

// Let's read an address from Firestore and 
// automatically convert it to our model.
let address = document<Address> "addresses" "POTUS-address"

// Again if your model inherits from "FirestoreDocument"
// you can use these features.
let docId = address.Id // => "POTUS-address""
let collectionId = address.CollectionId // => "addresses""

// --- or ---

// Let's just retrieve the document reference...
let addressRef = documentRef "addresses" "POTUS-address"
// And convert it to our model class manually.
let address = convertTo<Address> addressRef

// Again if your model inherits from "FirestoreDocument"
// you can use these features.
let docId = address.Id // => "POTUS-address""
let collectionId = address.CollectionId // => "addresses""
```

#### Querying Documents

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Firestore
open FsFirestore.Query

// To query a collection we first need to retrieve it from Firestore
let queryCollection = collection "addresses"

// Now we can chain conditions.
// Let's query all addresses in Pennsylvania Avenue, DC up to POTUS's one.
let addresses = 
    queryCollection
    |> orderBy "HouseNo"
    |> whereEqualTo "State" "DC"
    |> whereEqualTo "Street" "Pennsylvania Avenue"
    |> whereGreaterThenOrEqualTo "HouseNo" 1
    |> whereLessThenOrEqualTo "HouseNo" 1600
    |> execQuery<Address>
```

#### Writing Documents

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Firestore

// Let's create the model that we want to add to Firestore
let address = new Address()

// Now we can add the address to Firestore with a given 
// collection name and ID.
let docRef = addDocument "addresses" (Some "POTUS-address") address

// -- or --

// We can also add the address and let the ID be generated automatically.
let docRef = addDocument "addresses" None address
```

#### Updating Documents

```fsharp
open FsFirestore.Firestore

// We can first the read the document we want to update 
// from Firestore.
let address = document<Address> "addresses" "POTUS-address"

// Now let's move the presidents house number along one number.
address.HouseNo <- 1601

// And update the document within Firestore.
let docRef = updateDocument "addresses" "POTUS-address" address
```

#### Deleting  Documents

```fsharp
open FsFirestore.Firestore

// To delete a document we simply need the collection ID
// and the document ID.
deleteDocument None "addresses" "POTUS-address"

// -- or --

// Additionally we can specify a precondition for the deletion process.
let timeStamp = Timestamp.FromDateTime(DateTime.Today)
let precondition = Precondition.LastUpdated(timeStamp)
deleteDocument precondition "addresses" "POTUS-address"
```

### Transactions

Transactions are functions which take in a `Transaction` object to create, read, update, delete and query documents within the transaction scope.

##### Reading Documents in Transaction

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Transaction

// Reading a document works just as aspected only with the minor difference
// to use the transaction specific function from the Transaction module.
let transactionFunc (trans: Transaction) =
	documentInTrans<Address> trans "addresses" "POTUS-address"
	
// Now let's run the transaction.
// Notice that the transaction will return the return value from the actual transaction
// function.
let address = runTransaction transactionFunc

// -- or --

// You can specify the return values type, but F# will detected the type automatically
// most of the time.
let address = runTransaction<Address> transactionFunc
```

##### Writing Documents in Transaction

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Transaction

// Let's create the model that we want to add to Firestore
let address = new Address()

// Now let's write a transaction to add an address to Firestore with a given 
// collection name and ID.
let transactionFunc (trans: Transaction) =
	addDocumentInTrans trans "addresses" (Some "POTUS-address") address
	
// Let's run the transaction.
let docRef = runTransaction transactionFunc 

// -- or --

// Of course, we can also add the address and let the ID be generated automatically.
let transactionFunc (trans: Transaction) =
	addDocumentInTrans trans "addresses" None address
```

##### Updating Documents in Transaction

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Firestore

// Let's create our update transaction.
let transactionFunc (trans: Transaction) =
    // We can first read the document we want to update 
    // from Firestore.
	let address = documentInTrans<Address> trans "addresses" "POTUS-address"
	
    // Now let's move the presidents house number along one number.
    address.HouseNo <- 1601

    // And update the document.
    updateDocumentInTrans trans "addresses" "POTUS-address" address
    
// Let's run the transaction.
let docRef = runTransaction transactionFunc
```

##### Deleting Documents in Transaction

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Firestore

// Let's create our deletion transaction.
let transactionFunc (trans: Transaction) =
    // To delete a document we simply need the collection ID
    // and the document ID.
	deleteDocumentInTrans trans None "addresses" "POTUS-address"
	
// Let's run the transaction.
runTransaction transactionFunc

// -- or --

// As mentioned in the CRUD section we can also specify a precondition 
// for the deletion process.
let transactionFunc (trans: Transaction) =
    let timeStamp = Timestamp.FromDateTime(DateTime.Today)
    let precondition = Precondition.LastUpdated(timeStamp)
    deleteDocument precondition "addresses" "POTUS-address"
    
runTransaction transactionFunc
```

### Listening for Changes

Firestore provides the ability to use a streaming API that let's you listen to changes made do specific documents or a complete set of documents managed by a query.

The listener functions will be called on document creation, deletion and update. You can even specify a listener functions for none existing documents, this requires a named document ID before hand.

#### Data Definition

For the listening examples we will use different model classes.

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Types

// Stores scores per user, the username will be the document ID.
[<FirestoreData>]
type Score() =
	inherit FirestoreDocument() // Base class that comes with FsFirestore
	
	[<FirestoreProperty>]
	member val BestScore = 0 with get, set

	[<FirestoreProperty>]
	member val LastScore = 0 with get, set
	
// Stores a list of usernames that have high scores.
// A highscore is any score above 1000.
[<FirestoreData>]
type HighScores() =
	inherit FirestoreDocument() // Base class that comes with FsFirestore
	
	[<FirestoreProperty>]
	member val Usernames = [] with get, set
```

#### Mounting Listeners

```fsharp
open FsFirestore.Firestore
open FsFirestore.Listening

// To creater a listener it's as easy as writing a simple function,
// as it practically is just a function.
let callback (snap: DocumentSnapshot) =
	if snap.Exists = true then
		// The callback takes in a document snapshot, we can convert the snap
		// to our model.
		let score = convertSnapshotTo<Score> snap
		
		// Now we can use the listener to set the best score to the last score
		// if it was better then the current best score.
		if score.LastScore > score.BestScore then
			score.BestScore <- score.LastScore
			updateDocument score.CollectionId score.Ids score		
		
// Now we can simple mount our created listener callback and in 
// turn receive a istener object from Firestore
let listener = listenOnDocument "scores" "mrbandler" callback

// If we want to stop listening we just stop listening.
stopListening listener
```

#### Listening For Query Changes

```fsharp
open FsFirestore.Firestore
open FsFirestore.Listening

// Now we can use a listener to update a different document with the 
// best high scores.
let callback (querySnap: QuerySnapshot) =
	// If the query changes the callback is called and we can retrieve the
	// updated query results.
	let scores = convertSnapshotsTo<Score> querySnap.Documents |> List.ofSeq
	
	// Now we can extract the usernames from the scores into an array.
	let usernames =
        scores
        |> List.map (fun score -> score.Id)
        |> Array.ofList
	
	// Let's update our highscores document with the new usernames.
	let highScores = document<HighScores> "highscores" "users"
	highScores.Usernames <- usernames
	
	updateDocument highScores.CollectionId highScores.Id highScores
	
// Now let's mount our created listener callback to a query.
// We only want highscores that are above 1000, to be neat we also order them.
let listener =
	collection "scores"
	|> whereGreaterThen "BestScore" 1000
	|> orderBy "BestScore"
	|> listenOnQuery callback

// If we want to stop listening we just stop listening.
stopListening listener
```

In the above example we a simply retrieving all documents from the query which can be a lot of data. There is a way to only work with the changes from the query.

```fsharp
open FsFirestore.Firestore
open FsFirestore.Listening

// Now we can use a listener to update a different document with the 
// best high scores.
let callback (querySnap: QuerySnapshot) =
	// If the query changes the callback is called and we can retrieve the
	// updated query results.
	let scoreChanges = convertQueryChanges<Score> querySnap.Changes |> List.ofSeq
	
	// A document change contains a bit more data then a usual document
    let scoreChange = List.item 0
    let doc = scoreChange.document // => Actuall converted document data
    let changeType = scoreChange.changeType // => Added (there also is Updated and Removed)
    let newIndex = scoreChange.newIndex // => New index (option) if moved in the query
    let oldIndex = scoreChange.oldIndex // => Old index (option) if moved in the query
	
	// Now we can extract the usernames from the scores into an array.
	let usernames =
        scoreChanges
        |> List.filter (fun scoreChange -> scoreChange.changeType = DocumentChange.Type.Added)
        |> List.map (fun scoreChange -> scoreChange.document.Id)
        |> Array.ofList
	
	// Let's update our highscores document with the new usernames.
	let highScores = document<HighScores> "highscores" "users"
	Array.append highScores.Usernames usernames // Now instead of overwriting the all usernames we add to the array.
	updateDocument highScores.CollectionId highScores.Id highScores
	
// Now let's mount our created listener callback to a query.
// We only want highscores that are above 1000, to be neat we also order them.
let listener =
	collection "scores"
	|> whereGreaterThen "BestScore" 1000
	|> orderBy "BestScore"
	|> listenOnQuery callback

// If we want to stop listening we just stop listening.
stopListening listener
```

### Async Functions

> **TODO:** The async API will be added soon... Stay tuned!

## 2. Bugs and Features

Please open a issue when you encounter any bugs 🐞 or have an idea for a new feature 💡.

## 3. Buy me a coffee

If you like you can buy me a coffee:

[![Support via PayPal](https://cdn.rawgit.com/twolfson/paypal-github-button/1.0.0/dist/button.svg)](https://www.paypal.me/mrbandler/)

[![Donate with Bitcoin](https://en.cryptobadges.io/badge/big/3KGsDx52prxWciBkfNJYBkXaTJ6GUURP2c)](https://en.cryptobadges.io/donate/3KGsDx52prxWciBkfNJYBkXaTJ6GUURP2c)

[![Donate with Ethereum](https://en.cryptobadges.io/badge/big/0xd6Ffc89Bc87f7dFdf0ef1aefF956634d4B7451c8)](https://en.cryptobadges.io/donate/0xd6Ffc89Bc87f7dFdf0ef1aefF956634d4B7451c8)

---

## 4. License

MIT License

Copyright (c) 2018 fivefingergames

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.