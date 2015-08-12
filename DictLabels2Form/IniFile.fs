// Simple ini file reader/writer.
module CSPro.IniFile

type Key = {name : string
            value : string}

type Section = {name : string
                keys : Key list}

type IniFile = Sections of Section list

// Create empty ini file
let newIniFile =
    Sections []

// Create new ini file section with given name
let makeSection name =
    {name=name;keys=[]}

// Add a section to an ini file
let addSection (Sections ss) s = Sections(s::ss)

// Add a key to an ini file section
let addKey section key =
    {name=section.name;keys=key::section.keys}
   
// Get value corresponding to name in section
let getValueInSection name section =
    let key = List.find (fun {name=n;value=v} -> name = n) section.keys
    key.value

// Set value corresponding to name in section
let setValueInSection name value section =
    let newKeys = List.map (fun {name=n;value=v} -> if name = n then {name=n;value=value} else {name=n;value=v}) section.keys
    {name=section.name;keys=newKeys}

// Load ini file from disk
let loadIniFile filename =

    let readLines filePath = System.IO.File.ReadLines(filePath)

    let lines = readLines filename

    let (|Key|_|) str =
        let m = System.Text.RegularExpressions.Regex.Match(str, "(.*)=(.*)")
        if m.Success
        then Some {name=m.Groups.[1].Value;value=m.Groups.[2].Value}
        else None

    let parseKey str = 
        let m = System.Text.RegularExpressions.Regex.Match(str, "(.*)=(.*)")
        if m.Success
        then {name=m.Groups.[1].Value;value=m.Groups.[2].Value}
        else failwith ("Invalid key vaue pair: " + str)

    let addKeyToLastSection iniFile key =
        match iniFile with
            | (Sections (s::ss)) -> Sections((addKey s key)::ss)
            | (Sections []) -> failwith ("Found key outside of section: " + key.name)

    let (|SectionHeader|_|) str =
        let m = System.Text.RegularExpressions.Regex.Match(str, "\[(.*)\]")
        if m.Success
        then Some m.Groups.[1].Value
        else None

    let collectSections (Sections sections) line = 
        match line with
            | SectionHeader title -> title |> makeSection |> addSection (Sections sections)
            | keyline when keyline.Trim().Length = 0 -> (Sections sections)
            | keyline -> keyline |> parseKey |> addKeyToLastSection (Sections sections)

    let reverseKeys (Sections sections) =
        sections |> List.map (fun {name=n;keys=ks} -> {name=n;keys=List.rev ks}) |> Sections

    let reverseSections (Sections sections) = sections |> List.rev |> Sections

    Seq.fold collectSections newIniFile lines |> reverseKeys |> reverseSections 

// Write ini file to disk
let writeIniFile (filename:string) (Sections sections) =

    use writer = new System.IO.StreamWriter(filename)

    let writeKey (key:Key) =
        writer.WriteLine(key.name + "=" + key.value)

    let writeSection section =
        writer.WriteLine("[" + section.name + "]")
        List.iter writeKey section.keys
        writer.WriteLine()

    List.iter writeSection sections
