open System.IO
#r "nuget: FSharp.Json"
open FSharp.Json
#load "Types.fsx"
open Types

let isInProject (p:Project) r = p.Id = r.ProjectId
let getReleasesForProject p =
    let releases = File.ReadAllText "Data/Releases.json" |> Json.deserialize<Release list>
    List.filter (isInProject p) releases

let getLatestDeploymentForEnvAndRelease (e:Environment) (r:Release) =
    let deployments = File.ReadAllText "Data/Deployments.json" |> Json.deserialize<Deployment list>
    let deploymentsForEnvAndRelease = List.filter (fun d->d.EnvironmentId = e.Id && d.ReleaseId = r.Id) deployments
    match deploymentsForEnvAndRelease with
    | [] -> None
    | [d] -> Some d
    | _ -> Some(List.reduce (fun curr latest -> if curr.DeployedAt > latest.DeployedAt then curr else latest) deploymentsForEnvAndRelease)

let logReasonForRetention list =
    List.map (fun x->printfn $"{x.ReleaseId} kept because it was the most recently deployed to {x.EnvironmentId}") list |> ignore
    list

let keepNLatestReleases n (envAndReleases:EnvironmentAndReleases) =
    let env = envAndReleases |> fst
    envAndReleases
    |> snd
    |> List.map(getLatestDeploymentForEnvAndRelease(env))
    |> List.choose id
    |> List.sortByDescending (fun x->x.DeployedAt)
    |> List.truncate n
    |> logReasonForRetention
    |> List.map (fun x->x.ReleaseId)

let releaseRetention n =
    let projects = File.ReadAllText "Data/Projects.json" |> Json.deserialize<Project list>
    let environments = File.ReadAllText "Data/Environments.json" |> Json.deserialize<Environment list>
    List.map getReleasesForProject projects
        |> List.allPairs environments
        |> List.map(keepNLatestReleases(n))

let result = releaseRetention 1
0