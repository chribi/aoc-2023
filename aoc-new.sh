#!/usr/bin/bash

dotnet new console -o $1
dotnet add $1 reference LibAoc
dotnet sln add $1
cp template.cs "$1/Program.cs"
