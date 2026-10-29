
# Raamatukogu infosüsteem

Projekt on tehtud C# ja Entity Framework Core abil.

Rakenduses saab lisada raamatuid ja lugejaid,
otsida raamatuid, muuta ja kustutada raamatuid
ning laenutada ja tagastada raamatuid.

## Ülesanne 6

### 1. Milline on DbContext-i ülesanne?
DbContext ühendab rakenduse andmebaasiga.
Selle abil saab andmeid lisada, otsida, muuta ja kustutada.

### 2. Miks ei ole hea panna andmebaasipäringuid kasutajaliidese koodi?
Sest siis on kood keerulisem.
Kui andmebaasipäringud on eraldi, on koodi lihtsam muuta ja parandada.

### 3. Millisesse kihti paigutaksid andmebaasiga suhtlemise?
MVC arhitektuuris paigutaksin andmebaasiga suhtlemise
andmekihti (Data).
Nii on andmebaas ja kasutajaliides eraldatud.

## Raamatu kustutamine

Enne kustutamist tuleb kontrollida, kas raamatul on laenutusi.
Kui raamatul on laenutuste ajalugu, siis seda ei kustutata.
