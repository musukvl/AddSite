# AddSite
AddSite console utitlty allows to create sites in IIS 


```
usage: Musuk.AddSite hostName [/path:value] [/ip:value] [/port:number] [/pool:value]
    hostName        Host name for site binding. Site will have the same name too.
    [/path:value]   Site home directory path. Current directory by default
    [/ip:value]     Site binding IP. [127.0.0.1] by default
        default value: '127.0.0.1'
    [/port:number]  Site binding port. [80] by default.
        default value: 80
    [/pool:value]   Application pool for site. [.NET v4.5] by default
        default value: '.NET v4.5'
```

Sample usage:

```
D:\umbraco\starter5>addsite starter10.dev
Site was created.
Hosts file was recored appended.
Done.
```