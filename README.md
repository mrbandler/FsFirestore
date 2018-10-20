<img src="https://raw.githubusercontent.com/mrbandler/FsFirestore/develop/Icons/FsFirestoreTransparentBanner.png" alt="FsFirestore Icon Banner" width="150" height="110"/>

# FsFirestore 

[![pipeline status](https://gitlab.com/mrbandler/FsFirestore/badges/master/pipeline.svg)](https://gitlab.com/mrbandler/FsFirestore/commits/master) [![NuGet Badge](https://buildstats.info/nuget/FsFirestore?includePreReleases=true)](https://www.nuget.org/packages/FsFirestore)

**Functional F# library to access Firestore database via Google Cloud Platform (GCP) or Firebase Project.**

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
let fields = address.fields // => ["Pennsylvania Avenue"; 1600; "Washington"; "DC"]

// We can ask the model which ID and collection it belongs to.
let docId = address.id
let collectionId = address.collectionId
```

#### Reading Documents

```fsharp
open FsFirestore.Firestore

// Let's read an address from Firestore and 
// automatically convert it to our model.
let address = document<Address> "addresses" "POTUS-address"

// Again if your model inherits from "FirestoreDocument"
// you can use these features.
let docId = address.id // => "POTUS-address""
let collectionId = address.collectionId // => "addresses""

// --- or ---

// Let's just retrieve the document reference...
let addressRef = documentRef "addresses" "POTUS-address"
// And convert it to our model class manually.
let address = convertTo<Address> addressRef

// Again if your model inherits from "FirestoreDocument"
// you can use these features.
let docId = address.id // => "POTUS-address""
let collectionId = address.collectionId // => "addresses""
```

#### Querying Documents

```fsharp
open FsFirestore.Firestore
open FsFirestore.Query

// To query a collection we first need to retrieve it from
// Firestore
let queryCollection = collection "addresses"

// Now we can chain conditions togehter.
// Let's query all addresses in Pennsylvania Avenue, DC up to POTUS's one.
let addresses = 
	queryCollection
    |> whereEqualTo "State" "DC"
    |> whereEqualTo "Street" "Pennsylvania Avenue"
    |> whereGreaterThenOrEqualTo "HouseNo" 1
    |> whereLessThenOrEqualTo "HouseNo" 1600
    |> execQuery<Address>
```

#### Writing Documents

```fsharp
open FsFirestore.Firestore

// Let's create the model that we want to add to Firestore
let address = new Address()

// Now we can add the address to Firestore with a given 
// collection name and ID.
let docRef = addDocumentWithId "addresses" "POTUS-address" address

// -- or --

// We can also add the address and let the ID be generated automatically.
let docRef = addDocument "addresses" address
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
deleteDocument "addresses" "POTUS-address"
```

### Transactions

> **NOTE:** Transactions will be added soon...

### Listening for Document Changes (Streaming API)

> **NOTE:** The streaming API will be added soon...

## 3. Bugs and Features

Please open a issue when you encounter any bugs 🐞 or have an idea for a new feature 💡.

## 4. Buy me a coffee

If you like you can buy me a coffee:

[![Support via PayPal](https://cdn.rawgit.com/twolfson/paypal-github-button/1.0.0/dist/button.svg)](https://www.paypal.me/mrbandler/)

[![Donate with Bitcoin](https://en.cryptobadges.io/badge/big/3KGsDx52prxWciBkfNJYBkXaTJ6GUURP2c)](https://en.cryptobadges.io/donate/3KGsDx52prxWciBkfNJYBkXaTJ6GUURP2c)

[![Donate with Ethereum](https://en.cryptobadges.io/badge/big/0xd6Ffc89Bc87f7dFdf0ef1aefF956634d4B7451c8)](https://en.cryptobadges.io/donate/0xd6Ffc89Bc87f7dFdf0ef1aefF956634d4B7451c8)

---

## 5. License

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
