// Sync item labels in dictionary to labels and roster column headers
// on forms. Useful when you have made changes to labels in the dictionary
// and want to update the forms automatically.

open CSPro
open CSPro.IniFile

// Get label from name of item in CSPro dictionary
let labelFromDict (dict:DataDictionary) variableName =
    let getItemByName (dict:DataDictionary) name =
        let allRecTypes = dict.Levels |> Seq.collect (fun (l:CSPro.Level) -> l.Records)
        let allIdItems = dict.Levels |> Seq.map (fun (l:CSPro.Level) -> l.IDs)
        let allItems = allRecTypes |> Seq.append allIdItems |> Seq.collect (fun (r:CSPro.Record) -> r.Items)
        allItems |> Seq.tryFind (fun (i:CSPro.Item) -> i.Name = name)

    match getItemByName dict variableName with
        | Some item -> item.Label
        | None -> failwith ("Item " + variableName + " not found in dictionary")

let syncFieldLabels dict (Sections s) =

    let replaceFieldText (text, field) =
      let variableNameAndDict = getValueInSection "Item" field
      let variableName = variableNameAndDict.Split(',').[0]
      let label = labelFromDict dict variableName
      let newText = setValueInSection "Text" label text
      [newText;field]

    let folder sections section =
        match sections with
            | [] -> [section]
            | [last] -> [section;last]
            | last::rest -> match (section, last) with
                                | ({name="Text";keys=_}, {name="Field";keys=_}) as textAndField -> sections |> List.tail |> List.append (replaceFieldText textAndField)
                                | _ -> section::sections

    List.fold folder [] s |> List.rev |> Sections

let syncRosterColumnHeaders dict (Sections s) =

    let replaceHeaderText (field, header) =
      let variableNameAndDict = getValueInSection "Item" field
      let variableName = variableNameAndDict.Split(',').[0]
      let label = labelFromDict dict variableName
      let newHeader = setValueInSection "Text" label header
      [field;newHeader]

    let folder sections section =
        match sections with
            | [] -> [section]
            | [last] -> [section;last]
            | last::rest -> match (section, last) with
                                | ({name="Field";keys=_}, {name="HeaderText";keys=_}) as fieldAndHeader -> sections |> List.tail |> List.append (replaceHeaderText fieldAndHeader)
                                | _ -> section::sections

    List.fold folder [] s |> List.rev |> Sections

// Replace labels on forms with labels for corresponding items from dictionary.
// Replaces both regular form fields as well as header text in rosters.
let syncLabels dictFile =
    let dict = DataDictionaryReader.Load(dictFile) 
    (syncRosterColumnHeaders dict) >> (syncFieldLabels dict)

[<EntryPoint>]
let main argv = 
    match argv with
        | [|dict;inputform;outputform|] -> loadIniFile inputform
                                                |> syncLabels dict
                                                |> writeIniFile outputform
        | _ -> printfn "Usage: DictLabels2Form dict inputform outputform"
    0 // return an integer exit code
