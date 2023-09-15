#!/bin/bash

fable clean --yes

rm -r ./.parcel-cache/*
rm -r ./dist/*

echo ".parcel-cache and dist folders have been cleared!"