import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { map, Observable } from "rxjs";
import { Agent, UpdateAgentDto } from "../models/agent.model";
import { RegisterAgentDto } from "../models/auth-dto.model";
import { PagedResult } from "../models/paged-result.model";

@Injectable()
export class AgentService{
    private baseUrl = `${environment.apiUrl}/agents`;

    constructor(private http:HttpClient){}

    getAllAgents(params?: any):Observable<PagedResult<Agent>>{
        const cleanParams: any = {};
        for (const key in params) {
        if (params[key] !== '' && params[key] !== null && params[key] !== undefined) {
            cleanParams[key] = params[key];
        }
        }

        return this.http.get<any>(this.baseUrl,{params}).pipe(
            map(response=>{
                const mapped:PagedResult<Agent>={
                    pageNumber:response.pageNumber,
                    pageSize:response.pageSize,
                    totalCount:response.totalCount,
                    items:(response.items?.$values || []).map((item:any)=>this.transformAgent(item))
                };

                return mapped;
            })
        );
    }

    private transformAgent(raw:any):Agent{
        return {
            id:raw.id,
            agencyName:raw.agencyName,
            licenseNumber:raw.licenseNumber,
            phone:raw.phone,
            user:raw.user
        };
    }

    registerAgent(data:RegisterAgentDto):Observable<any>{
        return this.http.post(`${this.baseUrl}/register`,data);
    }

    updateAgent(id:string,data:UpdateAgentDto):Observable<any>{
        return this.http.put<Agent>(`${this.baseUrl}/${id}`,data);
    }

}