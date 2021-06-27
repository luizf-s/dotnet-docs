#!/bin/bash

dotnet publish

sudo ln -s $PWD/bin/Debug/net5.0/dnd /usr/bin/dnd
