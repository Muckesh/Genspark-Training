import { Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { ColorRequest, ColorResponse } from "../models/color.model";

@Injectable()
export class ColorService extends BaseService<ColorResponse,ColorRequest>{
    protected override endpoint="colors";
    
}