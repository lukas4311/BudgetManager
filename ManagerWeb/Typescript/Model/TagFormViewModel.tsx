import { TagModel } from "../ApiClient/Main/models";

export class TagFormViewModel {
    tags: TagModel[];
    tagId: number;
    onSave: (tagId: number) => void;
}
