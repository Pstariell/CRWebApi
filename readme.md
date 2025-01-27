# CRWebApi


## Lista WebApi
Method| Indirizzo   |Funzionalità
----  |   -----       |-----
GET   |    /api/Generate/xsd | Generazione XSD di Esempio (Effettua il salvataggio del modello in XSD nella directory **public/DataSet**)
GET |/api/Generate | Generazione modello XML di esempio da inviare attraverso il metodo post
POST | /api/Generate | Generazione Report tramite modello



## Esempio modello 
----
XML
```

<ModelGenerate>
<modello>CrystalReport1.rpt</modello>
<returnType>pdf</returnType>
<outputName>pdfFile</outputName>
<ds>
    <xs:schema id="dataSetTest_202111080905" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata"><xs:element name="dataSetTest_202111080905" msdata:IsDataSet="true" msdata:UseCurrentLocale="true"><xs:complexType><xs:choice minOccurs="0" maxOccurs="unbounded"><xs:element name="Table1"><xs:complexType><xs:sequence><xs:element name="nome" type="xs:string" minOccurs="0" /><xs:element name="cognome" type="xs:string" minOccurs="0" /></xs:sequence></xs:complexType></xs:element></xs:choice></xs:complexType></xs:element></xs:schema><dataSetTest_202111080905>
    <Table1>
        <nome>a</nome>
        <cognome>b0</cognome>
    </Table1>
    <Table1>
        <nome>a</nome>
        <cognome>b1</cognome>
    </Table1>
    <Table1>
        <nome>a</nome>
        <cognome>b2</cognome>
    </Table1>
    </dataSetTest_202111080905>
    </ds>
</ModelGenerate>
`````
