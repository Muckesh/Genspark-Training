import { Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { ModelRequest, ModelResponse } from "../models/model.model";

@Injectable()
export class ModelService extends BaseService<ModelResponse,ModelRequest>{
    protected override endpoint="models";
    
}