#!/bin/bash

dotnet publish

if [[ -x /usr/bin/dnd ]]; then
    sudo rm /usr/bin/dnd
fi

sudo ln -s $PWD/bin/Debug/net5.0/dnd /usr/bin/dnd
