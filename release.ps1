cd ./FsFirestore
dotnet pack -c Release -o ../package
cd ..
cp ./package/FsFirestore*.nupkg FsFirestore.nupkg
dotnet nuget push FsFirestore.nupkg -k $args[0] -s https://api.nuget.org/v3/index.json
