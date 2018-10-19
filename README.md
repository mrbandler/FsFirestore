<img src="https://raw.githubusercontent.com/mrbandler/FsFirestore/develop/Icons/FsFirestoreTransparentBanner.png" alt="FsFirestore Icon Banner" width="150" height="110"/>

# FsFirestore 

[![pipeline status](https://gitlab.com/mrbandler/FsFirestore/badges/master/pipeline.svg)](https://gitlab.com/mrbandler/FsFirestore/commits/master) [![NuGet Badge](https://buildstats.info/nuget/FsFirestore?includePreReleases=true)](https://www.nuget.org/packages/FsFirestore)

**Functional F# library to access Firestore database via Google Cloud Platform (GCP) or Firebase Project.**

## Table Of Content
1. [Installation](#1-installation) 💻
2. [Usage](#2-usage) ⌨️
3. [Bugs and Features](#3-bugs-and-features) 🐞💡
4. [Buy me a coffee](#4-buy-me-a-coffee) ☕
5. [License](#5-license) 📃

---

## 2. Usage

#### Connect to Firestore

To use any of the Firestore features you have to initialize the connection via a Service Account JSON (either for [Firebase](https://console.firebase.google.com/project/_/settings/serviceaccounts/adminsdk) or [GCP](https://cloud.google.com/docs/authentication/getting-started)).

```fsharp
open FsFirestore.Firestore

let connectToFirestore =
	let didWeConnect = initFirestore "./path/to/your/service_account.json"
```

The `initFirestore` function returns a boolean, to indicate whether the connection could be established.

#### Create, Read, Update and Delete (CRUD)

After your successfully connected to your Firestore DB you can start manipulating data.

For better handling within the API we added a generic retrieval functions, to simply retrieve your wanted model. Sadly the Googles .NET API only let's us use classes, to compensate we created a base class that can be used to make your life easier.

```fsharp
open Google.Cloud.Firestore
open FsFirestore.Types

[<FirestoreData>]
type Address() =
	inherit FirestoreDocument() // This is the base class that comes with FsFirestore
	
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

// We can retrieve all fields in a object list, which can later be used to query the DB.
let fields = address.fields // => ["Pennsylvania Avenue"; 1600; "Washington"; "DC"]

// We can ask the model, which in turn is a document in our Firebase DB which ID and Collection it belongs to, this is only true when retrieved via generic read function from FsFirestore.
let docId = address.id
let collectionId = address.collectionId
```

#### Let's read from the Firestore DB 

```fsharp
open FsFirestore.Firestore

// Let's read an address from the DB and automatically convert it to our model.
let address = document<Address> "addresses" "address-id"

let docId = address.id // => address-id
let collectionId = address.collectionId // => addresses

// ---

// Let's just retrieve the document reference...
let addressRef = documentRef "addresses" "address-id"
// When can if we wanted to then also convert it to our model type, so we would have the same as above.
let address = convertTo<Address> addressRef
```



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
