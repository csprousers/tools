# DictLabels2Form

Utility to sync labels on a CSPro form with corresponding item labels in the data dictionary. 

Useful when you have made a lot of changes to your dictionary labels and wan't make the same changes by hand to the forms. With CSPro 6.2 and up where you can link form labels to the dictionary this will not be needed.

## Usage

This is a console application so it is launched from the command line. Pass it the dictionary file, the input form file and the output form file. For each field in the input form file the associated label is replaced in the output file witht the label for the corresponding dictionary item.

For example:

```
DictLabels2Form mydict.dcf myform.fmf myupdatedform.fmf 
```

